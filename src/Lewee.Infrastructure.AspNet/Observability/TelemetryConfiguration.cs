using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Lewee.Infrastructure.AspNet.Observability;

/// <summary>
/// Telemetry Configuration
/// </summary>
public static class TelemetryConfiguration
{
    /// <summary>
    /// Adds OpenTelemetry metrics and tracing to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="webHostEnvironment">Web host environment</param>
    /// <param name="telemetryNamespace">Namespace to use to group related service in OpenTelemetry</param>
    /// <param name="telemetrytEndpoint">OpenTelemetry endpoint</param>
    /// <returns>Updated service collection</returns>
    public static IServiceCollection AddTelemetry(
        this IServiceCollection services,
        IWebHostEnvironment webHostEnvironment,
        string telemetryNamespace,
        string telemetrytEndpoint)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddTelemetrySdk()
                    .AddService(
                        serviceName: webHostEnvironment.ApplicationName,
                        serviceNamespace: telemetryNamespace,
                        serviceVersion: VersionHelper.GetVersion(),
                        serviceInstanceId: Environment.MachineName)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        { "deployment.environment", webHostEnvironment.EnvironmentName }
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter((exporterOptions, metricReaderOptions) =>
                    {
                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                    })
                    .AddOtlpExporter(builder => builder.Endpoint = new Uri(telemetrytEndpoint));
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(builder => builder.Endpoint = new Uri(telemetrytEndpoint));
            });

        return services;
    }
}
