using Microsoft.EntityFrameworkCore;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Api.Startup;

internal sealed class StoreDatabaseConfigurationService
{
    private readonly StoreDbContext dbContext;
    private readonly StartupStatusService startupStatusService;
    private readonly ILogger<StoreDatabaseConfigurationService> logger;

    public StoreDatabaseConfigurationService(
        StoreDbContext dbContext,
        StartupStatusService startupStatusService,
        ILogger<StoreDatabaseConfigurationService> logger)
    {
        this.dbContext = dbContext;
        this.startupStatusService = startupStatusService;
        this.logger = logger;
    }

    public bool IsReady => this.startupStatusService.IsDatabaseReady;

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Running database migrations...");

        await this.dbContext.Database.MigrateAsync(cancellationToken);
        this.startupStatusService.SetDatabaseMigrated();

        this.logger.LogInformation("Database migrations completed successfully");
    }

    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Seeding database...");

        var pizzas = Menu.Pizzas;
        var hasChanges = false;

        foreach (var item in pizzas)
        {
            var existing = await this.dbContext.Pizzas.FindAsync([item.Id], cancellationToken);
            if (existing == null)
            {
                this.dbContext.Pizzas.Add(item);
                hasChanges = true;
            }
        }

        if (!hasChanges)
        {
            this.logger.LogInformation("No seed data changes required");
            this.startupStatusService.SetDatabaseSeeded();

            return;
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.startupStatusService.SetDatabaseSeeded();

        this.logger.LogInformation("Database seeded successfully");
    }
}
