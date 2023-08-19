using Lewee.IntegrationTests;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Infrastructure.Data;
using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

public abstract class RestaurantTestsBase :
    WebApiIntegrationTests<Program, RestaurantWebApplicationFactory, RestaurantDbContextFixture, RestaurantDbContext>
{
    protected RestaurantTestsBase(RestaurantWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // TODO: This should be a collection fixture (https://github.com/TheMagnificent11/lewee/issues/23)
        var dbContext = this.GetService<RestaurantDbContext>();
        if (dbContext == null)
        {
            Assert.Fail("Can't seed Tables");
            return;
        }

        var seeder = new RestaurantDbSeeder(dbContext);

        await seeder.Seed(Table.DefaultData);

        await dbContext.SaveChangesAsync();
    }

    protected async Task AnEmptyRestaurant()
    {
        await this.HealthCheck("/health");
    }
}
