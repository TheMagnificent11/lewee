using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Pizzeria.Configuration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Use via DI")]
internal sealed class ConfigurationHealthCheck : IHealthCheck
{
    private readonly ConfigurationStatusService statusService;

    public ConfigurationHealthCheck(ConfigurationStatusService statusService)
    {
        this.statusService = statusService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (this.statusService.IsConfigurationComplete)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Configuration completed successfully"));
        }

        if (this.statusService.ConfigurationFailed)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Configuration failed"));
        }

        return Task.FromResult(HealthCheckResult.Degraded("Configuration in progress"));
    }
}
