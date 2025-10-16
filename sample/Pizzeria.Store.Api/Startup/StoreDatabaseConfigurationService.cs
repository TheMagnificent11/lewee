using Microsoft.EntityFrameworkCore;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Api.Startup;

internal sealed class StoreDatabaseConfigurationService
{
    private readonly StoreDbContext dbContext;
    private readonly ILogger<StoreDatabaseConfigurationService> logger;

    private bool isMigrated;
    private bool isSeeded;

    public StoreDatabaseConfigurationService(
        StoreDbContext dbContext,
        ILogger<StoreDatabaseConfigurationService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public bool IsReady => this.isMigrated && this.isSeeded;

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Running database migrations...");

        await this.dbContext.Database.MigrateAsync(cancellationToken);
        this.isMigrated = true;

        this.logger.LogInformation("Database migrations completed successfully");
    }

    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Seeding database...");

        var pizzas = Menu.Pizzas;
        var hasChanges = false;

        foreach (var item in pizzas)
        {
            var existing = await this.dbContext.Pizzas.FindAsync(item.Id, cancellationToken);

            if (existing == null)
            {
                this.dbContext.Pizzas.Add(item);
                hasChanges = true;
            }
        }

        if (!hasChanges)
        {
            this.logger.LogInformation("No seed data changes required");
            this.isSeeded = true;

            return;
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.isSeeded = true;

        this.logger.LogInformation("Database seeded successfully");
    }
}
