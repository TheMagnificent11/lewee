using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Tests.Integration;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
public sealed class PizzeriaApplicationFactory : IAsyncLifetime
{
    public const string CollectionName = "PizzeriaCollection";

    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;
    private ResourceNotificationService resourceNotificationService;
    private string keycloakBaseUrl;
    private IPlaywright playwright;

    private ServiceProvider serviceProvider;

    public async Task InitializeAsync()
    {
        Environments.SetToIntegrationTesting();

        // Initialize Playwright and ensure browsers are installed
        this.playwright = await Playwright.CreateAsync();

        // Install browsers if not already installed (for CI/CD environments)
        var exitCode = Program.Main(["install", "chromium"]);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Failed to install Playwright browsers. Exit code: {exitCode}");
        }

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
        using var authServerHttpClient = this.app.CreateHttpClient(ServiceNames.AuthServer);
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

        var databaseName = ServiceNames.PizzaStoreDatabaseName;
        var storeDbConnectionString = await this.app.GetConnectionStringAsync(databaseName);

        // Setup service provider
        var services = new ServiceCollection();
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseNpgsql(storeDbConnectionString);
        });
        services.AddTransient<IQueryProjectionService, QueryProjectionService<StoreDbContext>>();
        services.AddKeycloakAdminClient(
            Environments.Auth.RealmName,
            options => options.BaseAddress = new Uri(this.keycloakBaseUrl));

        this.serviceProvider = services.BuildServiceProvider();
    }

    // TODO: this should be removed after `PizzaOrderTests` are refactored to use Playwright
    public async Task<string> GetJwtAsync(string username, string password)
    {
        using var httpClient = new HttpClient();
        var tokenEndpoint = $"{this.keycloakBaseUrl}/realms/{Environments.Auth.RealmName}/protocol/openid-connect/token";

        using var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = Environments.Auth.Clients.StoreApi,
            ["username"] = username,
            ["password"] = password,
        });

        using var response = await httpClient.PostAsync(tokenEndpoint, tokenRequest);
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
        var storeDbContext = this.serviceProvider.GetRequiredService<StoreDbContext>();

        var order = await storeDbContext
            .Orders
            .OrderByDescending(x => x.ModifiedAtUtc)
            .FirstOrDefaultAsync();

        return order;
    }

    public async Task<Order> GetOrderAsync(Guid orderId)
    {
        var storeDbContext = this.serviceProvider.GetRequiredService<StoreDbContext>();

        var order = await storeDbContext
            .Orders
            .Include(x => x.Pizzas)
            .FirstOrDefaultAsync(x => x.Id == orderId);

        return order;
    }

    public async Task<User> GetLatestCustomerAsync()
    {
        var storeDbContext = this.serviceProvider.GetRequiredService<StoreDbContext>();

        var user = await storeDbContext
            .Users
            .OrderByDescending(x => x.ModifiedAtUtc)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetCustomerByExternalIdAsync(string externalId)
    {
        var storeDbContext = this.serviceProvider.GetRequiredService<StoreDbContext>();

        var user = await storeDbContext
            .Users
            .FirstOrDefaultAsync(x => x.ExternalId == externalId);

        return user;
    }

    public async Task<string> GetConnectionStringAsync(string serviceName)
    {
        return await this.app.GetConnectionStringAsync(serviceName);
    }

    public async Task<string> GetKeycloakUserIdAsync(string username)
    {
        var authClient = this.serviceProvider.GetRequiredService<IAuthServerAdminClient>();
        var userId = await authClient.GetUserIdAsync(username, CancellationToken.None);

        return userId;
    }

    public async Task<T> GetQueryProjectionAsync<T>(string key)
        where T : class, IQueryProjection
    {
        var storeDbQueryProjectionService = this.serviceProvider.GetRequiredService<IQueryProjectionService>();

        return await storeDbQueryProjectionService.RetrieveByKeyAsync<T>(key, CancellationToken.None);
    }

    public async Task<int> GetUndispatchedDomainEventCountAsync()
    {
        var storeDbContext = this.serviceProvider.GetRequiredService<StoreDbContext>();

        if (storeDbContext.DomainEventReferences == null)
        {
            return 0;
        }

        var count = await storeDbContext.DomainEventReferences
            .Where(x => !x.Dispatched)
            .CountAsync();

        return count;
    }

    public async Task<string> GetWebClientBaseUrlAsync()
    {
        using var httpClient = await this.GetServiceClientAsync(ServiceNames.PizzaStoreWebClient);
        var baseAddress = httpClient.BaseAddress!.ToString().TrimEnd('/');

        // If the scheme is tcp, replace it with http (Aspire might return tcp scheme)
        if (baseAddress.StartsWith("tcp://", StringComparison.OrdinalIgnoreCase))
        {
            baseAddress = $"http://{baseAddress[6..]}";
        }

        return baseAddress;
    }

    public Task<IPlaywright> GetPlaywrightAsync()
    {
        return Task.FromResult(this.playwright);
    }

    public async Task DisposeAsync()
    {
        this.playwright?.Dispose();

        if (this.serviceProvider != null)
        {
            await this.serviceProvider.DisposeAsync();
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
