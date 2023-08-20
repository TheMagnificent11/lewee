using Lewee.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Respawn;

namespace Lewee.IntegrationTests;

/// <summary>
/// Database Context Fixture
/// </summary>
/// <typeparam name="TDbContext">Database context type</typeparam>
/// <typeparam name="TDbSeeder">Database seeder type</typeparam>
public abstract class DatabaseContextFixture<TDbContext, TDbSeeder>
    where TDbContext : DbContext
    where TDbSeeder : IDatabaseSeeder<TDbContext>
{
    private bool isDbInitialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContextFixture{TDbContext, TDbSeeder}"/> class
    /// </summary>
    protected DatabaseContextFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{this.EnvironmentName}.json")
            .Build();

        this.ConnectionString = configuration.GetConnectionString(this.ConnectionStringName)
            ?? throw new InvalidOperationException("Could not find connection string");
    }

    /// <summary>
    /// Gets the database connection string
    /// </summary>
    protected string ConnectionString { get; }

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

    /// <summary>
    /// Resets the database
    /// </summary>
    /// <returns>An asynchronous task</returns>
    public async Task ResetDatabase()
    {
        var dbContext = this.CreateDbContext();
        var seeder = this.CreateDbSeeder(dbContext);

        if (!this.isDbInitialized)
        {
            await dbContext.Database.MigrateAsync();
            await seeder.Run();
            this.isDbInitialized = true;
            return;
        }

        try
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();
            }
        }
        catch (SqlException)
        {
            // Database doesn't exist yet, probably because migrations haven't been run to create it
            return;
        }

        var respawner = await Respawner.CreateAsync(this.ConnectionString, this.ResetOptions);
        await respawner.ResetAsync(this.ConnectionString);

        await seeder.Run();
    }

    /// <summary>
    /// Creates a database context
    /// </summary>
    /// <returns>Database context</returns>
    protected abstract TDbContext CreateDbContext();

    /// <summary>
    /// Create a database seeder
    /// </summary>
    /// <param name="context">Database context</param>
    /// <returns>Database seeder</returns>
    protected abstract TDbSeeder CreateDbSeeder(TDbContext? context = null);
}
