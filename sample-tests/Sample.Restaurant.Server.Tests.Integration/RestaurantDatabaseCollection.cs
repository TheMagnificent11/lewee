﻿using Lewee.IntegrationTests;
using Sample.Restaurant.Data;
using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

[CollectionDefinition("RestaurantDatabaseCollection")]
public sealed class RestaurantDatabaseCollection : DataCollection<
    RestaurantDbContextFixture,
    RestaurantDbContext,
    RestaurantDbSeeder>
{
}
