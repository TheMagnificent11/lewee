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

    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;
    private ResourceNotificationService resourceNotificationService;
    private StoreDbContext storeDbContext;
    private QueryProjectionService<StoreDbContext> storeDbQueryProjectionService;

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
}
