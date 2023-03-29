using Lewee.IntegrationTests;
using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

[Collection(nameof(RestaurantCollection))]
public abstract class RestaurantTestsBase : WebApiIntegrationTests<Program, RestaurantWebApplicationFactory>
{
    protected RestaurantTestsBase(RestaurantWebApplicationFactory factory)
        : base(factory)
    {
    }

    protected async Task AnEmptyRestaurant()
    {
        await this.HealthCheck("/health");
    }
}
