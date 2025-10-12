using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

namespace Pizzeria.Configuration.Services;

/// <summary>
/// Pizzeria store database configuration service
/// </summary>
public sealed class PizzeriaStoreDatabaseConfigurationService : IDatabaseConfigurationService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<PizzeriaStoreDatabaseConfigurationService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PizzeriaStoreDatabaseConfigurationService"/> class.
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <param name="logger">Logger</param>
    public PizzeriaStoreDatabaseConfigurationService(
        IConfiguration configuration,
        ILogger<PizzeriaStoreDatabaseConfigurationService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task MigrateAsync()
    {
        this.logger.LogInformation("Running database migrations...");

        var databaseName = ServiceNames.GetPizzaStoreDatabaseName();
        var connectionString = this.configuration.GetConnectionString(databaseName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string for '{databaseName}' not found");
        }

        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        await using var dbContext = new StoreDbContext(optionsBuilder.Options);
        await dbContext.Database.MigrateAsync();

        this.logger.LogInformation("Database migrations completed successfully");
    }

    /// <inheritdoc/>
    public async Task SeedDataAsync()
    {
        this.logger.LogInformation("Seeding database...");

        var databaseName = ServiceNames.GetPizzaStoreDatabaseName();
        var connectionString = this.configuration.GetConnectionString(databaseName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string for '{databaseName}' not found");
        }

        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        await using var dbContext = new StoreDbContext(optionsBuilder.Options);

        var pizzas = Menu.Pizzas;
        var hasChanges = false;

        foreach (var item in pizzas)
        {
            var existing = await dbContext.Pizzas.FindAsync(item.Id);

            if (existing == null)
            {
                dbContext.Pizzas.Add(item);
                hasChanges = true;
            }
        }

        if (!hasChanges)
        {
            this.logger.LogInformation("No seed data changes required");
            return;
        }

        await dbContext.SaveChangesAsync();
        this.logger.LogInformation("Database seeded successfully");
    }
}
