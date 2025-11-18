using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Tests.Integration;

public sealed class PizzeriaApplicationFactory : IAsyncLifetime
{
    public const string CollectionName = "PizzeriaCollection";

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
        this.builder.Services.ConfigureHttpClientDefaults(x => { });

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

        // Wait for configuration to be running (it's now a web app, not a console app that finishes)
        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.ConfigurationService, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));

        // Wait for configuration health check to report healthy
        await this.WaitForConfigurationHealthAsync(TimeSpan.FromMinutes(2));

        // Wait for API to be running
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

    public async Task<string> GetJwtAsync()
    {
        using var httpClient = new HttpClient();
        var tokenEndpoint = $"{this.keycloakBaseUrl}/realms/{Environments.Auth.RealmName}/protocol/openid-connect/token";

        var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = Environments.Auth.ApiClientId,
            ["username"] = Environments.Auth.Users.Customer1.Username,
            ["password"] = Environments.Auth.Users.Customer1.Password,
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

    public async Task<User> GetLatestCustomerAsync()
    {
        var user = await this.storeDbContext
            .Users
            .OrderByDescending(x => x.ModifiedAtUtc)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetCustomerByExternalIdAsync(string externalId)
    {
        var user = await this.storeDbContext
            .Users
            .FirstOrDefaultAsync(x => x.ExternalId == externalId);

        return user;
    }

    public async Task<string> GetConnectionStringAsync(string serviceName)
    {
        return await this.app.GetConnectionStringAsync(serviceName);
    }

    public async Task<string> CreateKeycloakUserAsync(string username, string password)
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(this.keycloakBaseUrl) };
        using var memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(
            new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
        var authClient = AuthServerServiceCollectionExtensions.CreateAuthServerAdminClient(
            httpClient,
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance,
            memoryCache);

        // Create the user
        await authClient.CreateUserAsync(Environments.Auth.RealmName, username, password, CancellationToken.None);

        // Get the user ID
        var userId = await authClient.GetUserIdAsync(Environments.Auth.RealmName, username, CancellationToken.None);

        return userId;
    }

    public async Task<string> GetKeycloakUserIdAsync(string username)
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(this.keycloakBaseUrl) };
        using var memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(
            new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
        var authClient = AuthServerServiceCollectionExtensions.CreateAuthServerAdminClient(
            httpClient,
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance,
            memoryCache);

        // Get the user ID
        var userId = await authClient.GetUserIdAsync(Environments.Auth.RealmName, username, CancellationToken.None);

        return userId;
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

    private async Task WaitForConfigurationHealthAsync(TimeSpan timeout)
    {
        using var httpClient = this.app.CreateHttpClient(ServiceNames.ConfigurationService);
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                var response = await httpClient.GetAsync("/health");
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
                // Ignore exceptions and continue polling
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new TimeoutException($"Configuration health check timed out after {timeout.TotalMinutes} minutes. The /health endpoint did not return a successful response.");
    }
}
