using Lewee.IntegrationTests;
using NUnit.Framework;

namespace Sample.Restaurant.Server.Tests.Integration;

public abstract class RestaurantTestsBase : WebApiIntegrationTests<Program, RestaurantWebApplicationFactory>
{
    private readonly RestaurantDbContextFixture dbContextFixture;

    protected RestaurantTestsBase()
    {
        this.dbContextFixture = new RestaurantDbContextFixture();
    }

    [SetUp]
    public async Task Setup()
    {
        await this.dbContextFixture.ResetDatabase();
    }

    protected async Task AnEmptyRestaurant()
    {
        await this.HealthCheck("/health");
    }
}
