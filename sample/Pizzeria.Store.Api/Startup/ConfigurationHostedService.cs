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
            await using var scope = this.serviceProvider.CreateAsyncScope();

            var keycloakConfigService = scope.Resolve<AuthServerConfigurationService>();

            await this.SetupKeycloakAsync(keycloakConfigService, stoppingToken);
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ex, "Failed to configure auth server");

            // Even if configuration fails, mark as ready to allow startup to complete
            // The actual API calls will fail later with authentication errors
            try
            {
                await using var scope = this.serviceProvider.CreateAsyncScope();
                var startupStatusService = scope.Resolve<StartupStatusService>();
                startupStatusService.SetKeycloakReady();
                this.logger.LogWarning("Marked Keycloak as ready despite configuration failure");
            }
            catch (Exception statusEx)
            {
                this.logger.LogError(statusEx, "Failed to mark startup status as ready");
            }
        }
    }

    private async Task SetupKeycloakAsync(
        AuthServerConfigurationService keycloakConfigurationService,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Setting-up auth server...");

        await keycloakConfigurationService.ConfigureAsync(cancellationToken);

        this.logger.LogInformation("Setting-up auth server...complete");
    }
}
