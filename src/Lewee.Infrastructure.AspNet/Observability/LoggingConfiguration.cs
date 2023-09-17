using Lewee.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Lewee.Infrastructure.AspNet.Observability;

/// <summary>
/// Logging Configuration
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension method to configure logging
    /// </summary>
    /// <param name="builder">
    /// Host builder
    /// </param>
    /// <param name="configuration">
    /// Configuration
    /// </param>
    /// <param name="webHostEnvironment">
    /// Web host environment
    /// </param>
    /// <returns>
    /// Serilog logger
    /// </returns>
    public static Logger ConfigureLogging(
        this IHostBuilder builder,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        var seqSettings = configuration.GetSettings<SeqSettings>(nameof(SeqSettings));

        var serilogLevelSwitch = new LoggingLevelSwitch
        {
            MinimumLevel = seqSettings?.MinimumLogEventLevel ?? LogEventLevel.Information
        };

        var config = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(serilogLevelSwitch)
            .WriteTo.Trace()
            .WriteTo.Console()
            .Enrich.WithProperty("ApplicationName", webHostEnvironment.ApplicationName)
            .Enrich.WithProperty("Environment", webHostEnvironment.EnvironmentName)
            .Enrich.WithProperty("Version", VersionHelper.GetVersion())
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.FromLogContext();

        if (seqSettings != null && seqSettings.IsEnabled)
        {
            var seqLevelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = seqSettings.MinimumLogEventLevel
            };

            config.WriteTo.Seq(seqSettings.Uri, apiKey: seqSettings.Key, controlLevelSwitch: seqLevelSwitch);
        }

        var logger = config.CreateLogger();

        logger.Information(
            "================= {ApplicationName} Started =================",
            webHostEnvironment.ApplicationName);

        return logger;
    }
}
