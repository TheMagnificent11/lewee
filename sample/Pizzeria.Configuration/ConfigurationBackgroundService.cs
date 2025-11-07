using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pizzeria.Configuration;

internal sealed class ConfigurationBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ConfigurationBackgroundService> logger;
    private readonly ConfigurationStatusService statusService;

    public ConfigurationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ConfigurationBackgroundService> logger,
        ConfigurationStatusService statusService)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.statusService = statusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Pizzeria Configuration starting...");

        try
        {
            using var scope = this.serviceProvider.CreateScope();

            // Configure database first
            var dbConfigService = scope.ServiceProvider.GetRequiredService<PizzeriaStoreDatabaseConfigurationService>();
            await dbConfigService.ConfigureAsync(stoppingToken);

            // Then configure Keycloak
            var keycloakConfigService = scope.ServiceProvider.GetRequiredService<KeycloakConfigurationService>();
            await keycloakConfigService.ConfigureAsync(stoppingToken);

            this.statusService.SetConfigurationComplete();
            this.logger.LogInformation("✅ Pizzeria Configuration completed successfully");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "❌ Pizzeria Configuration failed: {Message}", ex.Message);
            this.statusService.SetConfigurationFailed();
        }
    }
}
