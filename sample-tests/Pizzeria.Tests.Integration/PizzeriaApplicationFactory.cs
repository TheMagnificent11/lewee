using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
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

    public async Task InitializeAsync()
    {
        // https://learn.microsoft.com/en-us/dotnet/aspire/testing/manage-app-host?pivots=xunit
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Pizzeria_AppHost>();
        this.builder.Services.ConfigureHttpClientDefaults(x =>
        {
            x.AddStandardResilienceHandler();
        });

        this.app = await this.builder.BuildAsync();
        this.resourceNotificationService= this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.PizzaStoreApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(1));

        var storeDbConnectionString = await this.app.GetConnectionStringAsync(ServiceNames.PizzaStoreDatabase);
        var storeDbOptionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        storeDbOptionsBuilder.UseNpgsql(storeDbConnectionString);

        this.storeDbContext = new StoreDbContext(storeDbOptionsBuilder.Options);
    }

    public async Task<HttpClient> GetServiceClientAsync(string serviceName)
    {
        var client = this.app.CreateHttpClient(serviceName);

        await this.resourceNotificationService
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(10));

        return client;
    }

    public async Task<Order> GetLatestOrder()
    {
        var order = await this.storeDbContext
            .Orders
            .OrderByDescending(x => x.ModifiedAtUtc)
            .FirstOrDefaultAsync();

        return order;
    }

    public async Task<Order> GetOrder(Guid orderId)
    {
        var order = await this.storeDbContext
            .Orders
            .Include(x => x.Pizzas)
            .FirstOrDefaultAsync(x => x.Id == orderId);

        return order;
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
