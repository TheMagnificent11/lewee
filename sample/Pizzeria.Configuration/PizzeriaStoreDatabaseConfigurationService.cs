using Lewee.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Data;

namespace Pizzeria.Configuration;

internal sealed class PizzeriaStoreDatabaseConfigurationService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<PizzeriaStoreDatabaseConfigurationService> logger;

    public PizzeriaStoreDatabaseConfigurationService(
        IServiceProvider serviceProvider,
        ILogger<PizzeriaStoreDatabaseConfigurationService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task ConfigureAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken; // Unused but kept for consistency with interface

        try
        {
            this.logger.LogInformation("Starting database migration and seeding...");

            await this.serviceProvider.MigrateDatabaseAsync<StoreDbContext>(seedData: true);

            this.logger.LogInformation("✅ Database migration and seeding completed successfully");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "❌ Database configuration failed: {Message}", ex.Message);
            throw;
        }
    }
}
