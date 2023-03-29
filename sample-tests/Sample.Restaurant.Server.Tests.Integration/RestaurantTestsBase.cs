using Lewee.IntegrationTests;
using Respawn;
using Respawn.Graph;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public abstract class RestaurantTestsBase : WebApiIntegrationTests<Program, RestaurantWebApplicationFactory>
{
    private const string RestuarantDbSchema = "res";

    protected RestaurantTestsBase(RestaurantWebApplicationFactory factory)
        : base(factory)
    {
    }

    protected override DatabaseResetConfiguration[] TestDatabases => new[]
    {
        new DatabaseResetConfiguration(
            typeof(RestaurantDbContext),
            new RespawnerOptions
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
            })
    };

    protected async Task AnEmptyRestaurant()
    {
        await this.ResetDatabase<RestaurantDbContext>();
    }
}
