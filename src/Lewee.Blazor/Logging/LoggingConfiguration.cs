using Serilog;

namespace Lewee.Blazor.Logging;

/// <summary>
/// Logging Configuration
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configures logging
    /// </summary>
    /// <param name="loggingInjestionUrl">Logging injestion URL</param>
    public static void ConfigureLogging(string loggingInjestionUrl)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.BrowserConsole()
            .WriteTo.BrowserHttp($"{loggingInjestionUrl}injest")
            .CreateLogger();

        Log.Information("Started client-side application");
    }
}
