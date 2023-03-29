using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Respawn;
using Xunit;

namespace Lewee.IntegrationTests;

/// <summary>
/// Database Context Fixture
/// </summary>
/// <typeparam name="T">Database context type</typeparam>
public abstract class DatabaseContextFixture<T> : IAsyncLifetime
    where T : DbContext
{
    private readonly string connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContextFixture{T}"/> class
    /// </summary>
    protected DatabaseContextFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{this.EnvironmentName}.json")
            .Build();

        var connectionString = configuration.GetConnectionString(this.ConnectionStringName)
            ?? throw new InvalidOperationException("Could not find connection string");

        this.connectionString = connectionString;
    }

    /// <summary>
    /// Gets the database reset options
    /// </summary>
    protected abstract RespawnerOptions ResetOptions { get; }

    /// <summary>
    /// Gets the environment name
    /// </summary>
    protected abstract string EnvironmentName { get; }

    /// <summary>
    /// Gets the connection string name used in appsettings.json
    /// </summary>
    protected abstract string ConnectionStringName { get; }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        try
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
            }
        }
        catch (SqlException)
        {
            // Database doesn't exist yet, probably because migrations haven't been run to create it
            return;
        }

        var respawner = await Respawner.CreateAsync(this.connectionString, this.ResetOptions);
        await respawner.ResetAsync(this.connectionString);
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
