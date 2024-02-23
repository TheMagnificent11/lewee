using Lewee.IntegrationTests;
using Sample.Restaurant.Infrastructure.Data;
using Xunit;

namespace Sample.Restaurant.Tests.Integration;

[CollectionDefinition("RestaurantDatabaseCollection")]
public sealed class RestaurantDatabaseCollection : DataCollection<
    RestaurantDbContextFixture,
    RestaurantDbContext,
    RestaurantDbSeeder>
{
}
