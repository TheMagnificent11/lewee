using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

[CollectionDefinition(nameof(RestaurantCollection))]
public class RestaurantCollection : ICollectionFixture<RestaurantDbContextFixture>
{
}
