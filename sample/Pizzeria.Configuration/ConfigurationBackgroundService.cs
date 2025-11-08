namespace Pizzeria.Configuration;

internal sealed class ConfigurationBackgroundService : BackgroundService
{
    private readonly PizzeriaStoreDatabaseConfigurationService dbConfigService;
    private readonly AuthServerConfigurationService authServerConfigService;
    private readonly ConfigurationStatusService statusService;
    private readonly ILogger<ConfigurationBackgroundService> logger;

    public ConfigurationBackgroundService(
        PizzeriaStoreDatabaseConfigurationService dbConfigService,
        AuthServerConfigurationService authServerConfigService,
        ConfigurationStatusService statusService,
        ILogger<ConfigurationBackgroundService> logger)
    {
        this.dbConfigService = dbConfigService;
        this.authServerConfigService = authServerConfigService;
        this.statusService = statusService;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Pizzeria Configuration starting...");

        try
        {
            // Configure database first
            await this.dbConfigService.ConfigureAsync(stoppingToken);

            // Then configure Keycloak
            await this.authServerConfigService.ConfigureAsync(stoppingToken);

            this.statusService.SetConfigurationComplete();
            this.logger.LogInformation("Pizzeria Configuration completed successfully");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Pizzeria Configuration failed: {Message}", ex.Message);
            this.statusService.SetConfigurationFailed();
        }
    }
}
