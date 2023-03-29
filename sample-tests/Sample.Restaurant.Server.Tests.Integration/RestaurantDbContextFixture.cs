using Lewee.IntegrationTests;
using Respawn;
using Respawn.Graph;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public sealed class RestaurantDbContextFixture : DatabaseContextFixture<RestaurantDbContext>
{
    private const string RestuarantDbSchema = "res";

    protected override RespawnerOptions ResetOptions => new()
    {
        DbAdapter = DbAdapter.SqlServer,
        SchemasToExclude = new[] { "dbo" },
        TablesToIgnore = new[]
        {
            new Table(RestuarantDbSchema, nameof(RestaurantDbContext.DomainEventReferences)),
            new Table(RestuarantDbSchema, nameof(RestaurantDbContext.Tables)),
            new Table(RestuarantDbSchema, nameof(RestaurantDbContext.MenuItems)),
            new Table(RestuarantDbSchema, "OrderStatuses"),
            new Table(RestuarantDbSchema, "MenuItemTypes")
        }
    };
}
