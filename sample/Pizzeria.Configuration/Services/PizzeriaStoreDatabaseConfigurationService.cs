using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

namespace Pizzeria.Configuration.Services;

public sealed class PizzeriaStoreDatabaseConfigurationService : IPizzeriaStoreDatabaseConfigurationService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<PizzeriaStoreDatabaseConfigurationService> logger;

    public PizzeriaStoreDatabaseConfigurationService(
        IConfiguration configuration,
        ILogger<PizzeriaStoreDatabaseConfigurationService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task ConfigureAsync()
    {
        this.logger.LogInformation("Starting database configuration...");

        var databaseName = ServiceNames.GetPizzaStoreDatabaseName();
        var connectionString = this.configuration.GetConnectionString(databaseName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string for '{databaseName}' not found");
        }

        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        await using var dbContext = new StoreDbContext(optionsBuilder.Options);

        this.logger.LogInformation("Running database migrations...");
        await dbContext.Database.MigrateAsync();

        this.logger.LogInformation("Seeding database...");
        await this.SeedDatabaseAsync(dbContext);

        this.logger.LogInformation("Database configuration completed successfully");
    }

    private async Task SeedDatabaseAsync(StoreDbContext dbContext)
    {
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
