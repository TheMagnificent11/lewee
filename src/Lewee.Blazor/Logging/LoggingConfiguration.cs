// TODO: https://github.com/TheMagnificent11/lewee/issues/15
//using Serilog;

//namespace Lewee.Blazor.Logging;

///// <summary>
///// Logging Configuration
///// </summary>
//public static class LoggingConfiguration
//{
//    /// <summary>
//    /// Configures logging
//    /// </summary>
//    /// <param name="loggingIngestionUrl">Logging ingestion URL</param>
//    public static void ConfigureLogging(string loggingIngestionUrl)
//    {
//        Log.Logger = new LoggerConfiguration()
//            .MinimumLevel.Debug()
//            .WriteTo.BrowserConsole()
//            .WriteTo.BrowserHttp($"{loggingIngestionUrl}ingest")
//            .CreateLogger();

//        Log.Information("Started client-side application");
//    }
//}
