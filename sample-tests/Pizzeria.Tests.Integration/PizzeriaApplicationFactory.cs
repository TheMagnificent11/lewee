using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Tests.Integration;

public sealed class PizzeriaApplicationFactory : IAsyncLifetime
{
    public const string CollectionName = "PizzeriaCollection";
    private const string TestUsername = "testuser";
    private const string TestPassword = "testpassword";

    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;
    private ResourceNotificationService resourceNotificationService;
    private StoreDbContext storeDbContext;
    private QueryProjectionService<StoreDbContext> storeDbQueryProjectionService;
    private string keycloakBaseUrl;

    public async Task InitializeAsync()
    {
        Environments.SetToIntegrationTesting();

        // https://learn.microsoft.com/en-us/dotnet/aspire/testing/manage-app-host?pivots=xunit
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Pizzeria_AppHost>();
        this.builder.Services.ConfigureHttpClientDefaults(x =>
        {
            x.AddStandardResilienceHandler();
        });

        this.app = await this.builder.BuildAsync();
        this.resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        // Wait for auth server to be running
        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.AuthServer, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(10)); // To allow Aspire to pull Docker images

        // Get auth server base URL for token requests using CreateHttpClient
        var authServerHttpClient = this.app.CreateHttpClient(ServiceNames.AuthServer);
        var baseAddress = authServerHttpClient.BaseAddress!.ToString().TrimEnd('/');

        // If the scheme is tcp, replace it with http (Aspire Keycloak might return tcp scheme)
        if (baseAddress.StartsWith("tcp://", StringComparison.OrdinalIgnoreCase))
        {
            baseAddress = "http://" + baseAddress.Substring(6);
        }

        this.keycloakBaseUrl = baseAddress;

        // Initialize Keycloak realm and test user
        await this.InitializeKeycloakAsync();

        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.PizzaStoreApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(10)); // To allow Aspire to pull Docker images

        var databaseName = ServiceNames.GetPizzaStoreDatabaseName();
        var storeDbConnectionString = await this.app.GetConnectionStringAsync(databaseName);
        var storeDbOptionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();

        storeDbOptionsBuilder.UseNpgsql(storeDbConnectionString);

        this.storeDbContext = new StoreDbContext(storeDbOptionsBuilder.Options);
        this.storeDbQueryProjectionService = new QueryProjectionService<StoreDbContext>(this.storeDbContext);
    }

    public async Task<string> GetJwtTokenAsync()
    {
        using var httpClient = new HttpClient();
        var tokenEndpoint = $"{this.keycloakBaseUrl}/realms/pizzeria/protocol/openid-connect/token";

        var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = "pizzeria-store-api",
            ["username"] = TestUsername,
            ["password"] = TestPassword
        });

        var response = await httpClient.PostAsync(tokenEndpoint, tokenRequest);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
        return tokenResponse!.AccessToken;
    }

    public async Task<HttpClient> GetServiceClientAsync(string serviceName)
    {
        var client = this.app.CreateHttpClient(serviceName);

        await this.resourceNotificationService
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(10));

        return client;
    }

    public async Task<Order> GetLatestOrderAsync()
    {
        var order = await this.storeDbContext
            .Orders
            .OrderByDescending(x => x.ModifiedAtUtc)
            .FirstOrDefaultAsync();

        return order;
    }

    public async Task<Order> GetOrderAsync(Guid orderId)
    {
        var order = await this.storeDbContext
            .Orders
            .Include(x => x.Pizzas)
            .FirstOrDefaultAsync(x => x.Id == orderId);

        return order;
    }

    public async Task<string> GetConnectionStringAsync(string serviceName)
    {
        return await this.app.GetConnectionStringAsync(serviceName);
    }

    public async Task<T> GetQueryProjectionAsync<T>(string key)
        where T : class, IQueryProjection
    {
        return await this.storeDbQueryProjectionService.RetrieveByKeyAsync<T>(key, CancellationToken.None);
    }

    public async Task<int> GetUndispatchedDomainEventCountAsync()
    {
        if (this.storeDbContext.DomainEventReferences == null)
        {
            return 0;
        }

        var count = await this.storeDbContext.DomainEventReferences
            .Where(x => !x.Dispatched)
            .CountAsync();

        return count;
    }

    public async Task DisposeAsync()
    {
        if (this.storeDbContext != null)
        {
            await this.storeDbContext.DisposeAsync();
        }

        if (this.app != null)
        {
            await this.app.StopAsync();
            await this.app.DisposeAsync();
        }

        if (this.builder != null)
        {
            await this.builder.DisposeAsync();
        }
    }

    private async Task InitializeKeycloakAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(this.keycloakBaseUrl);

        // Get admin token
        var adminTokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = "admin",
            ["password"] = "admin"
        });

        var adminTokenResponse = await httpClient.PostAsync("/realms/master/protocol/openid-connect/token", adminTokenRequest);
        adminTokenResponse.EnsureSuccessStatusCode();
        var adminToken = await adminTokenResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken!.AccessToken);

        // Create pizzeria realm
        var realmPayload = new
        {
            realm = "pizzeria",
            enabled = true,
            sslRequired = "none"
        };

        var realmResponse = await httpClient.PostAsJsonAsync("/admin/realms", realmPayload);
        if (!realmResponse.IsSuccessStatusCode)
        {
            var error = await realmResponse.Content.ReadAsStringAsync();
            if (!error.Contains("Conflict", StringComparison.Ordinal))
            {
                throw new Exception($"Failed to create realm: {error}");
            }
        }

        // Create client
        var clientPayload = new
        {
            clientId = "pizzeria-store-api",
            enabled = true,
            publicClient = true,
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            redirectUris = new[] { "*" },
            webOrigins = new[] { "*" },
            attributes = new { access_token_lifespan = "300" }
        };

        var clientResponse = await httpClient.PostAsJsonAsync("/admin/realms/pizzeria/clients", clientPayload);
        if (!clientResponse.IsSuccessStatusCode)
        {
            var error = await clientResponse.Content.ReadAsStringAsync();
            if (!error.Contains("Conflict", StringComparison.Ordinal))
            {
                throw new Exception($"Failed to create client: {error}");
            }
        }

        // Create test user
        var userPayload = new
        {
            username = TestUsername,
            enabled = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = TestPassword,
                    temporary = false
                }
            }
        };

        var userResponse = await httpClient.PostAsJsonAsync("/admin/realms/pizzeria/users", userPayload);
        if (!userResponse.IsSuccessStatusCode)
        {
            var error = await userResponse.Content.ReadAsStringAsync();
            if (!error.Contains("Conflict", StringComparison.Ordinal) && !error.Contains("already exists", StringComparison.Ordinal))
            {
                throw new Exception($"Failed to create user: {error}");
            }
        }
    }
}
