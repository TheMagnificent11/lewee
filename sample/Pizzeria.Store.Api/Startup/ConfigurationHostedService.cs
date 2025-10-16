using FastEndpoints;

namespace Pizzeria.Store.Api.Startup;

internal sealed class ConfigurationHostedService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ConfigurationHostedService> logger;

    public ConfigurationHostedService(
        IServiceProvider serviceProvider,
        ILogger<ConfigurationHostedService> logger)
        : base()
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = this.serviceProvider.CreateScope();

            var dbConfigService = scope.Resolve<StoreDatabaseConfigurationService>();
            var keycloakConfigService = scope.Resolve<KeycloakConfigurationService>();

            await this.MigrateDatabaseAsync(dbConfigService, stoppingToken);
            await this.SeedDatabaseAsync(dbConfigService, stoppingToken);
            await this.SetupKeycloakAsync(keycloakConfigService, stoppingToken);
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ex, "Failed to configure database/auth server");
        }
    }

    private async Task MigrateDatabaseAsync(
        StoreDatabaseConfigurationService databaseConfigurationService,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Migrating database...");

        await databaseConfigurationService.MigrateAsync(cancellationToken);

        this.logger.LogInformation("Migrating database...complete");
    }

    private async Task SeedDatabaseAsync(
        StoreDatabaseConfigurationService databaseConfigurationService,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Seeding database...");

        await databaseConfigurationService.SeedDataAsync(cancellationToken);

        this.logger.LogInformation("Seeding database...complete");
    }

    private async Task SetupKeycloakAsync(
        KeycloakConfigurationService keycloakConfigurationService,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Setting-up auth server...");

        await keycloakConfigurationService.ConfigureAsync(cancellationToken);

        this.logger.LogInformation("Setting-up auth server...complete");
    }
}
