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

#pragma warning disable CA1822 // Mark members as static
    protected Task AnEmptyRestaurant()
#pragma warning restore CA1822 // Mark members as static
    {
        return Task.CompletedTask;
    }
}
