using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Configuration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Use via DI")]
internal sealed class ConfigurationBackgroundService : BackgroundService
{
    private readonly PizzeriaStoreDatabaseConfigurationService dbConfigService;
    private readonly ConfigurationStatusService statusService;
    private readonly ILogger<ConfigurationBackgroundService> logger;

    public ConfigurationBackgroundService(
        PizzeriaStoreDatabaseConfigurationService dbConfigService,
        ConfigurationStatusService statusService,
        ILogger<ConfigurationBackgroundService> logger)
    {
        this.dbConfigService = dbConfigService;
        this.statusService = statusService;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Pizzeria Configuration starting...");

        try
        {
            await this.dbConfigService.ConfigureAsync(stoppingToken);

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
