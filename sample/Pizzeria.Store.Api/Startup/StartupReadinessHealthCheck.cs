using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Pizzeria.Store.Api.Startup;

internal sealed class StartupReadinessHealthCheck : IHealthCheck
{
    private readonly StartupStatusService startupStatusService;

    public StartupReadinessHealthCheck(StartupStatusService startupStatusService)
    {
        this.startupStatusService = startupStatusService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isKeycloakReady = this.startupStatusService.IsKeycloakReady;

        if (isKeycloakReady)
        {
            return Task.FromResult(HealthCheckResult.Healthy("All startup services are ready"));
        }

        var details = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["keycloak"] = isKeycloakReady ? "ready" : "not ready",
        };

        var description = $"Startup services not ready - Keycloak: {(isKeycloakReady ? "ready" : "not ready")}";

        return Task.FromResult(HealthCheckResult.Unhealthy(description, data: details));
    }
}
