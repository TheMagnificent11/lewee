using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal static partial class TimedOperationLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Beginning operation {TimedOperation}")]
    public static partial void LogBeginningOperation(this ILogger logger, string timedOperation);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Completed operation {TimedOperation} in {TimedOperationElapsedMs} ms")]
    public static partial void LogCompletedOperation(this ILogger logger, string timedOperation, long timedOperationElapsedMs);
}
