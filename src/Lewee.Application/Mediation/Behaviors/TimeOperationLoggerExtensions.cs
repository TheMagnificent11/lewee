using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal static class TimeOperationLoggerExtensions
{
    public static IDisposable BeginTimedOperation(this ILogger logger, string operationName)
    {
        return new TimedOperation(logger, operationName);
    }
}
