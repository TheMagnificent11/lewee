using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Lewee.Infrastructure.Logging;

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
    /// <returns>
    /// Serilog logger
    /// </returns>
    public static Logger ConfigureLogging(this IHostBuilder builder, IConfiguration configuration)
    {
        var applicationSettings = configuration.GetValue<ApplicationSettings>(nameof(ApplicationSettings));
        var seqSettings = configuration.GetValue<SeqSettings>(nameof(SeqSettings));

        if (applicationSettings == null)
        {
            throw new InvalidOperationException("ApplicationSettings do not exist in appsettings.json");
        }

        var serilogLevelSwitch = new LoggingLevelSwitch
        {
            MinimumLevel = seqSettings?.MinimumLogEventLevel ?? LogEventLevel.Information
        };

        var config = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(serilogLevelSwitch)
            .WriteTo.Trace()
            .WriteTo.Console()
            .Enrich.WithProperty("ApplicationName", applicationSettings.Name)
            .Enrich.WithProperty("Environment", applicationSettings.Environment)
            .Enrich.WithProperty("Version", applicationSettings.Version)
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

        logger.Information("================= {ApplicationName} Started =================", applicationSettings.Name);

        return logger;
    }
}
