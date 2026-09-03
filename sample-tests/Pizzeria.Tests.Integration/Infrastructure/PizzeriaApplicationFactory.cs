using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Auth.Domain;
using Lewee.Auth.Infrastructure.Data;
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

namespace Pizzeria.Tests.Integration.Infrastructure;

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
        var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Failed to install Playwright browsers. Exit code: {exitCode}");
        }

        // https://learn.microsoft.com/en-us/dotnet/aspire/testing/manage-app-host?pivots=xunit
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Pizzeria_AppHost>();
        this.app = await this.builder.BuildAsync().WaitAsync(TimeSpan.FromMinutes(5));
        this.resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        // Bound DCP startup so a stuck DCP process fails fast with a clear timeout
        // instead of hanging the test run indefinitely (see https://github.com/TheMagnificent11/lewee/issues/505)
        await this.app.StartAsync().WaitAsync(TimeSpan.FromMinutes(10));

        // Wait for auth server to be running
        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.AuthServer, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(10)); // To allow Aspire to pull Docker images

        // Get auth server base URL for token requests using CreateHttpClient
        using var authServerHttpClient = this.app.CreateHttpClient(ServiceNames.AuthServer);
        var baseAddress = authServerHttpClient.BaseAddress.ToString().TrimEnd('/');

        // If the scheme is tcp, replace it with http (Aspire Keycloak might return tcp scheme)
        if (baseAddress.StartsWith("tcp://", StringComparison.OrdinalIgnoreCase))
        {
            baseAddress = $"http://{baseAddress[6..]}";
        }

        this.keycloakBaseUrl = baseAddress;

        // Wait for configuration console app to finish running database migrations and seeding
        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.ConfigurationService, KnownResourceStates.Finished)
            .WaitAsync(TimeSpan.FromMinutes(5));

        // Wait for Pizza Store API to be running
        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.PizzaStoreApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(10)); // To allow Aspire to pull Docker images

        // Wait for Pizza Store Web to be running
        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.PizzaStoreWeb, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(10)); // To allow Aspire to pull Docker images

        var databaseName = ServiceNames.PizzaStoreDatabaseName;
        var databaseConnectionString = await this.app.GetConnectionStringAsync(databaseName);

        // Setup service provider
        var services = new ServiceCollection();
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseNpgsql(databaseConnectionString);
        });
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(databaseConnectionString);
        });
        services.AddTransient<IQueryProjectionService, QueryProjectionService<StoreDbContext>>();
        services.AddKeycloakAdminClient(
            Environments.Auth.RealmName,
            options => options.BaseAddress = new Uri(this.keycloakBaseUrl));

        this.serviceProvider = services.BuildServiceProvider();
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
        var authDbContext = this.serviceProvider.GetRequiredService<AuthDbContext>();

        var user = await authDbContext
            .Users
            .OrderByDescending(x => x.ModifiedAtUtc)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetCustomerByExternalIdAsync(string externalId)
    {
        var authDbContext = this.serviceProvider.GetRequiredService<AuthDbContext>();

        var user = await authDbContext
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
        using var httpClient = await this.GetServiceClientAsync(ServiceNames.PizzaStoreWeb);
        var baseAddress = httpClient.BaseAddress.ToString().TrimEnd('/');

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
}
