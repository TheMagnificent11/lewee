using Npgsql;
using Pizzeria.Common;
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
        var connectionString = await this.factory.GetConnectionStringAsync(ServiceNames.GetPizzaStoreDatabaseName(Environments.IntegrationTesting));
        
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        this.respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["Pizzas"] // Don't reset the Pizzas table as it contains seed data
        });

        await this.respawner.ResetAsync(connection);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}