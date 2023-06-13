using Lewee.IntegrationTests;
using NUnit.Framework;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public abstract class RestaurantTestsBase : WebApiIntegrationTests<Program, RestaurantWebApplicationFactory>
{
    private readonly RestaurantDbContextFixture dbContextFixture;

    protected RestaurantTestsBase()
    {
        this.dbContextFixture = new RestaurantDbContextFixture();
    }

    [SetUp]
    public virtual async Task Setup()
    {
        await this.dbContextFixture.ResetDatabase();

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
