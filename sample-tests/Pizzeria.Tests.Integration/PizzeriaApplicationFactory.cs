using System.Net.Http.Headers;
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
    private const string TestUsername = "test-user";
    private const string TestPassword = "test-password";

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
            baseAddress = $"http://{baseAddress[6..]}";
        }

        this.keycloakBaseUrl = baseAddress;

        // Wait for Keycloak to be ready by polling its health endpoint
        await this.WaitForKeycloakReadyAsync();

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
        var tokenEndpoint = $"{this.keycloakBaseUrl}/realms/{Environments.Keycloak.RealmName}/protocol/openid-connect/token";

        var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = Environments.Keycloak.ApiClientId,
            ["username"] = TestUsername,
            ["password"] = TestPassword,
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

    private static async Task<string> GetAdminAccessTokenAsync(HttpClient httpClient)
    {
        using var adminTokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = Environments.Keycloak.IntegrationTesting.AdminUsername,
            ["password"] = Environments.Keycloak.IntegrationTesting.AdminPassword,
        });

        using var adminTokenResponse = await httpClient.PostAsync(
            "/realms/master/protocol/openid-connect/token",
            adminTokenRequest);

        if (adminTokenResponse.IsSuccessStatusCode)
        {
            var adminToken = await adminTokenResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

            return adminToken!.AccessToken;
        }
        else
        {
            var errorContent = await adminTokenResponse.Content.ReadAsStringAsync();
            throw new Exception($"Failed to authenticate with Keycloak. Status: {adminTokenResponse.StatusCode}, Error: {errorContent}");
        }
    }

    private static async Task CreatePizzeriaRealmAsync(HttpClient httpClient)
    {
        var realmPayload = new
        {
            realm = Environments.Keycloak.RealmName,
            enabled = true,
            sslRequired = "none",
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
    }

    private static async Task CreateStoreApiClientAsync(HttpClient httpClient)
    {
        var clientPayload = new
        {
            clientId = Environments.Keycloak.ApiClientId,
            enabled = true,
            publicClient = true,
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            redirectUris = new[] { "*" },
            webOrigins = new[] { "*" },
            attributes = new { access_token_lifespan = "300" },
        };

        using var clientResponse = await httpClient.PostAsJsonAsync(
            $"/admin/realms/{Environments.Keycloak.RealmName}/clients",
            clientPayload);

        if (!clientResponse.IsSuccessStatusCode)
        {
            var error = await clientResponse.Content.ReadAsStringAsync();
            if (!error.Contains("Conflict", StringComparison.Ordinal))
            {
                throw new Exception($"Failed to create client: {error}");
            }
        }
    }

    private static async Task CreateTestUserAsync(HttpClient httpClient)
    {
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
                    temporary = false,
                },
            },
        };
        var userResponse = await httpClient.PostAsJsonAsync(
            $"/admin/realms/{Environments.Keycloak.RealmName}/users",
            userPayload);
        if (!userResponse.IsSuccessStatusCode)
        {
            var error = await userResponse.Content.ReadAsStringAsync();
            if (!error.Contains("Conflict", StringComparison.Ordinal) && !error.Contains("already exists", StringComparison.Ordinal))
            {
                throw new Exception($"Failed to create user: {error}");
            }
        }
    }

    private async Task WaitForKeycloakReadyAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(this.keycloakBaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        var maxAttempts = 30; // 30 attempts with 2-second delays = up to 1 minute
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                // Try to access the Keycloak master realm endpoint
                var response = await httpClient.GetAsync("/realms/master");
                if (response.IsSuccessStatusCode)
                {
                    // Keycloak is ready
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Keycloak not ready yet
            }
            catch (TaskCanceledException)
            {
                // Timeout, Keycloak not ready yet
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new Exception("Keycloak did not become ready within the timeout period");
    }

    private async Task InitializeKeycloakAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(this.keycloakBaseUrl);

        var adminAccessToken = await GetAdminAccessTokenAsync(httpClient);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAccessToken);

        await CreatePizzeriaRealmAsync(httpClient);
        await CreateStoreApiClientAsync(httpClient);
        await CreateTestUserAsync(httpClient);
    }
}
