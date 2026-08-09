using System.Diagnostics.CodeAnalysis;
using Npgsql;
using Pizzeria.Common;
using Pizzeria.Tests.Integration.Infrastructure;
using Respawn;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Trait("Category", "Aspire")]
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
public abstract class PizzeriaTests : IAsyncLifetime
{
    protected const string TableExistsSql = @"
SELECT EXISTS
(
    SELECT 1
    FROM information_schema.tables
    WHERE table_schema NOT IN ('pg_catalog','information_schema')
);";

    protected PizzeriaTests(PizzeriaApplicationFactory factory)
    {
        this.Factory = factory;
    }

    protected PizzeriaApplicationFactory Factory { get; }

    public async Task InitializeAsync()
    {
        var databaseName = ServiceNames.PizzaStoreDatabaseName;
        var connectionString = await this.Factory.GetConnectionStringAsync(databaseName);

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
            var undispatchedCount = await this.Factory.GetUndispatchedDomainEventCountAsync();

            if (undispatchedCount == 0)
            {
                return;
            }

            await Task.Delay(delay);
        }

        var finalCount = await this.Factory.GetUndispatchedDomainEventCountAsync();
        throw new TimeoutException($"Timed out waiting for domain events to be dispatched. {finalCount} events remain undispatched after {timeout.TotalSeconds} seconds.");
    }

    private static async Task<bool> TablesExistsAsync(NpgsqlConnection connection)
    {
        await using var command = new NpgsqlCommand(TableExistsSql, connection);

        var anyTablesExist = (bool)(await command.ExecuteScalarAsync());

        return anyTablesExist;
    }
}
