using Lewee.IntegrationTests;
using Sample.Restaurant.Infrastructure.Data;
using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

[Collection("RestaurantDatabaseCollection")]
public abstract class RestaurantTestsBase :
    WebApiIntegrationTests<Program, RestaurantWebApplicationFactory, RestaurantDbContextFixture, RestaurantDbContext, RestaurantDbSeeder>
{
    protected RestaurantTestsBase(
        RestaurantWebApplicationFactory webApplicationFactory,
        RestaurantDbContextFixture dbContextFixture)
        : base(webApplicationFactory, dbContextFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
    }

    protected async Task AnEmptyRestaurant()
    {
        await this.HealthCheck("/health");
    }
}
