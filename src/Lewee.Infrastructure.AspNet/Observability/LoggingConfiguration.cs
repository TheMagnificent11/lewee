using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
    /// Configures logging
    /// </summary>
    /// <param name="webHostEnvironment">Web host environment</param>
    /// <param name="configuration">Configuration</param>
    /// <returns> Serilog logger </returns>
    public static Logger ConfigureLogging(
        this IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration)
    {
        var config = ConfigureCommonLogging(configuration, webHostEnvironment);

        var logger = config.CreateLogger();

        logger.Information(
            "================= {ApplicationName} Started =================",
            webHostEnvironment.ApplicationName);

        return logger;
    }

    // TODO: https://github.com/TheMagnificent11/lewee/issues/15
    //public static Logger ConfigureLoggingWithBrowserInjestion(
    //    this IWebHostEnvironment webHostEnvironment,
    //    IConfiguration configuration,
    //    string baseAddress)
    //{
    //    var config = ConfigureCommonLogging(configuration, webHostEnvironment);

    //    config.WriteTo.BrowserHttp($"{baseAddress}ingest");

    //    var logger = config.CreateLogger();

    //    logger.Information(
    //        "================= {ApplicationName} Started =================",
    //        webHostEnvironment.ApplicationName);

    //    return logger;
    //}

    private static LoggerConfiguration ConfigureCommonLogging(
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
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = "http://localhost:4317/v1/logs";
                options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = webHostEnvironment.ApplicationName
                };
            })
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

        return config;
    }
}
