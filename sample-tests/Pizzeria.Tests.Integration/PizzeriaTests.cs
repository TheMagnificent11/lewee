using Npgsql;
using Pizzeria.Common;
using Respawn;
using Xunit;

namespace Pizzeria.Tests.Integration;

public abstract class PizzeriaTests : IAsyncLifetime
{
    protected const string TableExistsSql = @"
SELECT EXISTS
(
    SELECT 1
    FROM information_schema.tables
    WHERE table_schema NOT IN ('pg_catalog','information_schema')
);";

#pragma warning disable SA1401 // Field should be private - This field must be protected to be accessible by derived test classes
    protected readonly PizzeriaApplicationFactory factory;
#pragma warning restore SA1401

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
            TablesToIgnore = ["Pizzas"], // Don't reset the Pizzas table as it contains seed data
        });

        await respawner.ResetAsync(connection);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task WaitForDomainEventsToBeDispatchedAsync()
    {
        var timeout = TimeSpan.FromSeconds(30);
        var delay = TimeSpan.FromSeconds(5);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            var undispatchedCount = await this.factory.GetUndispatchedDomainEventCountAsync();

            if (undispatchedCount == 0)
            {
                return;
            }

            await Task.Delay(delay);
        }

        var finalCount = await this.factory.GetUndispatchedDomainEventCountAsync();
        throw new TimeoutException($"Timed out waiting for domain events to be dispatched. {finalCount} events remain undispatched after {timeout.TotalSeconds} seconds.");
    }

    private static async Task<bool> TablesExistsAsync(NpgsqlConnection connection)
    {
        await using var command = new NpgsqlCommand(TableExistsSql, connection);

        var anyTablesExist = (bool)(await command.ExecuteScalarAsync())!;

        return anyTablesExist;
    }
}
