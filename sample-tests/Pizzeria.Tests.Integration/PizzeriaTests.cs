using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Respawn;
using Xunit;

namespace Pizzeria.Tests.Integration;

public abstract class PizzeriaTests : IAsyncLifetime
{
    protected const string tableExistsSql = @"
SELECT EXISTS
(
    SELECT 1
    FROM information_schema.tables
    WHERE table_schema NOT IN ('pg_catalog','information_schema')
);";

    protected readonly PizzeriaApplicationFactory factory;

    protected PizzeriaTests(PizzeriaApplicationFactory factory)
    {
        this.factory = factory;
    }

    public async Task InitializeAsync()
    {
        var databaseName = ServiceNames.GetPizzaStoreDatabaseName();
        var connectionString = await this.factory.GetConnectionStringAsync(databaseName);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var tablesExists = await TablesExistsAsync(connection);
        if (!tablesExists)
        {
            return;
        }

        var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["Pizzas"] // Don't reset the Pizzas table as it contains seed data
        });

        await respawner.ResetAsync(connection);
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

    private static async Task<bool> TablesExistsAsync(NpgsqlConnection connection)
    {
        await using var command = new NpgsqlCommand(tableExistsSql, connection);

        var anyTablesExist = (bool)(await command.ExecuteScalarAsync())!;

        return anyTablesExist;
    }
}
