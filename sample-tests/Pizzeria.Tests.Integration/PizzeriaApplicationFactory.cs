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
    private DistributedApplication _app;
    private ResourceNotificationService resourceNotificationService;
    private StoreDbContext storeDbContext;

    internal DistributedApplication App => this._app;

    public async Task InitializeAsync()
    {
        // Set the environment to IntegrationTesting for integration tests BEFORE creating the builder
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.IntegrationTesting);

        // https://learn.microsoft.com/en-us/dotnet/aspire/testing/manage-app-host?pivots=xunit
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Pizzeria_AppHost>();
        this.builder.Services.ConfigureHttpClientDefaults(x =>
        {
            x.AddStandardResilienceHandler();
        });

        this._app = await this.builder.BuildAsync();
        this.resourceNotificationService= this._app.Services.GetRequiredService<ResourceNotificationService>();

        await this._app.StartAsync();

        await this.resourceNotificationService
            .WaitForResourceAsync(ServiceNames.PizzaStoreApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(10)); // To allow Aspire to pull Docker images

        var storeDbConnectionString = await this._app.GetConnectionStringAsync(ServiceNames.GetPizzaStoreDatabaseName(Environments.IntegrationTesting));
        var storeDbOptionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        storeDbOptionsBuilder.UseNpgsql(storeDbConnectionString);

        this.storeDbContext = new StoreDbContext(storeDbOptionsBuilder.Options);
    }

    public async Task<HttpClient> GetServiceClientAsync(string serviceName)
    {
        var client = this._app.CreateHttpClient(serviceName);

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

    public async Task<string> GetConnectionStringAsync(string serviceName)
    {
        return await this._app.GetConnectionStringAsync(serviceName);
    }

    public async Task DisposeAsync()
    {
        if (this.storeDbContext != null)
        {
            await this.storeDbContext.DisposeAsync();
        }

        if (this._app != null)
        {
            await this._app.StopAsync();
            await this._app.DisposeAsync();
        }

        if (this.builder != null)
        {
            await this.builder.DisposeAsync();
        }
    }
}
