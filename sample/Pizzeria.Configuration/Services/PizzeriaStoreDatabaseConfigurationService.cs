using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

namespace Pizzeria.Configuration.Services;

/// <summary>
/// Pizzeria store database configuration service
/// </summary>
public sealed class PizzeriaStoreDatabaseConfigurationService : IDatabaseConfigurationService
{
    private readonly StoreDbContext dbContext;
    private readonly ILogger<PizzeriaStoreDatabaseConfigurationService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PizzeriaStoreDatabaseConfigurationService"/> class.
    /// </summary>
    /// <param name="dbContext">Store database context</param>
    /// <param name="logger">Logger</param>
    public PizzeriaStoreDatabaseConfigurationService(
        StoreDbContext dbContext,
        ILogger<PizzeriaStoreDatabaseConfigurationService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task MigrateAsync()
    {
        this.logger.LogInformation("Running database migrations...");
        await this.dbContext.Database.MigrateAsync();
        this.logger.LogInformation("Database migrations completed successfully");
    }

    /// <inheritdoc/>
    public async Task SeedDataAsync()
    {
        this.logger.LogInformation("Seeding database...");

        var pizzas = Menu.Pizzas;
        var hasChanges = false;

        foreach (var item in pizzas)
        {
            var existing = await this.dbContext.Pizzas.FindAsync(item.Id);

            if (existing == null)
            {
                this.dbContext.Pizzas.Add(item);
                hasChanges = true;
            }
        }

        if (!hasChanges)
        {
            this.logger.LogInformation("No seed data changes required");
            return;
        }

        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Database seeded successfully");
    }
}
