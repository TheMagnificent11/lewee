using System.Text.Json;
using System.Text.Json.Serialization;
using Lewee.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lewee.Infrastructure.HealthChecks;

/// <summary>
/// Health Check Response Writer
/// </summary>
public class HealthCheckResponseWriter
{
    private readonly ApplicationSettings applicationSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckResponseWriter"/> class
    /// </summary>
    /// <param name="applicationSettings">
    /// Application settings
    /// </param>
    public HealthCheckResponseWriter(ApplicationSettings applicationSettings)
    {
        this.applicationSettings = applicationSettings;
    }

    /// <summary>
    /// Writes to HTTP response
    /// </summary>
    /// <param name="httpContext">
    /// HTTP context
    /// </param>
    /// <param name="healthReport">
    /// Health report
    /// </param>
    /// <returns>
    /// Asychronous task
    /// </returns>
    public async Task WriteToResponse(HttpContext httpContext, HealthReport healthReport)
    {
        if (httpContext is null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        if (healthReport is null)
        {
            throw new ArgumentNullException(nameof(healthReport));
        }

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.Headers["x-app-name"] = this.applicationSettings.Name;
        httpContext.Response.Headers["x-app-environment"] = this.applicationSettings.Environment;
        httpContext.Response.Headers["x-release-version"] = this.applicationSettings.Version;

        await httpContext.Response.WriteAsync(GenerateContent(healthReport));
    }

    private static string GenerateContent(HealthReport healthReport)
    {
        var healthy = GetDescriptionsForStatus(healthReport, HealthStatus.Healthy);
        var errors = GetDescriptionsForStatus(healthReport, HealthStatus.Unhealthy);
        var degraded = GetDescriptionsForStatus(healthReport, HealthStatus.Degraded);

        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        var value = new
        {
            errors,
            degraded,
            healthy,
            status = healthReport.Status.ToString(),
        };

        return JsonSerializer.Serialize(value, jsonOptions);
    }

    private static Dictionary<string, string> GetDescriptionsForStatus(HealthReport healthReport, HealthStatus status)
    {
        var map = healthReport.Entries
            .Where(e => e.Value.Status == status)
            .ToDictionary(p => p.Key, p => EntryDescription(p.Value));

        return map.Count == 0 ? new Dictionary<string, string>() : map;
    }

    private static string EntryDescription(HealthReportEntry entry)
    {
        if (!string.IsNullOrEmpty(entry.Description))
        {
            return entry.Description;
        }

#pragma warning disable CS8603 // Possible null reference return. The analyzer is wrong; the Status enum cannot be null and there's a default in the switch statement.
        return entry.Status switch
        {
            HealthStatus.Healthy => $"OK - {entry.Duration.TotalSeconds:0.000}s",
            HealthStatus.Degraded => entry.Exception != null ? $"Warning - {entry.Exception.Message}" : "Warning",
            HealthStatus.Unhealthy => entry.Exception != null ? $"Failure - {entry.Exception.Message}" : "Failure",
            _ => entry.ToString(),
        };
#pragma warning restore CS8603 // Possible null reference return.
    }
}
