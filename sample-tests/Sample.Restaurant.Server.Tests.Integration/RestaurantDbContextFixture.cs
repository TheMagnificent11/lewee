using Lewee.IntegrationTests;
using Respawn;
using Respawn.Graph;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public sealed class RestaurantDbContextFixture : DatabaseContextFixture<RestaurantDbContext>
{
    private const string RestaurantDbSchema = "res";

    protected override RespawnerOptions ResetOptions => new()
    {
        DbAdapter = DbAdapter.SqlServer,
        SchemasToExclude = new[] { "dbo" },
        TablesToIgnore = new[]
        {
            new Table(RestaurantDbSchema, nameof(RestaurantDbContext.MenuItems)),
            new Table(RestaurantDbSchema, "OrderStatuses"),
            new Table(RestaurantDbSchema, "MenuItemTypes")
        }
    };

    protected override string EnvironmentName => "Testing";

    protected override string ConnectionStringName => "Sample.Restaurant";
}
