using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Respawn;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public abstract class PizzeriaTests : IAsyncLifetime
{
    protected readonly PizzeriaApplicationFactory factory;
    private Respawner respawner;

    protected PizzeriaTests(PizzeriaApplicationFactory factory)
    {
        this.factory = factory;
    }

    public async Task InitializeAsync()
    {
        var connectionString = await this.factory.GetConnectionStringAsync(ServiceNames.GetPizzaStoreDatabaseName());
        
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Ensure database is migrated before using Respawn
        await EnsureDatabaseMigrated(connectionString);

        this.respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["Pizzas"] // Don't reset the Pizzas table as it contains seed data
        });

        await this.respawner.ResetAsync(connection);
    }

    private static async Task EnsureDatabaseMigrated(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        await using var dbContext = new StoreDbContext(optionsBuilder.Options);
        
        // Apply any pending migrations
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}