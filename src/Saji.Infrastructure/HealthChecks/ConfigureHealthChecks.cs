using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Saji.Infrastructure.Settings;

namespace Saji.Infrastructure.HealthChecks;

/// <summary>
/// Configure Health Checks
/// </summary>
public static class ConfigureHealthChecks
{
    /// <summary>
    /// Get health check options
    /// </summary>
    /// <param name="configuration">
    /// Configuration
    /// </param>
    /// <param name="applicationSettingsSectionName">
    /// Application settings section name
    /// </param>
    /// <returns>
    /// Health check options
    /// </returns>
    public static HealthCheckOptions GetHealthCheckOptions(
        this IConfiguration configuration,
        string applicationSettingsSectionName)
    {
        if (string.IsNullOrWhiteSpace(applicationSettingsSectionName))
        {
            throw new ArgumentException(
                $"'{nameof(applicationSettingsSectionName)}' cannot be null or whitespace",
                nameof(applicationSettingsSectionName));
        }

        var appSettings = configuration.GetSettings<ApplicationSettings>(applicationSettingsSectionName);
        var responseWriter = new HealthCheckResponseWriter(appSettings);

        return new HealthCheckOptions
        {
            ResponseWriter = responseWriter.WriteToResponse
        };
    }
}
