using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

internal static partial class RequestEffectsLogMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Executing query request...success")]
    public static partial void LogQueryRequestSucceeded(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Executing query request...error (Error Message: {ErrorMessage})")]
    public static partial void LogQueryRequestFailed(this ILogger logger, string errorMessage);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Executing request...")]
    public static partial void LogRequestExecuting(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "An error occurred while executing the query request: {ErrorMessage}")]
    public static partial void LogRequestExecutionFailed(this ILogger logger, Exception exception, string errorMessage);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Executing query request...completed")]
    public static partial void LogQueryRequestCompleted(this ILogger logger);
}
