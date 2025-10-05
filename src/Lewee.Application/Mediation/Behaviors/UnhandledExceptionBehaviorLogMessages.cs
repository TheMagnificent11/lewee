using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal static partial class UnhandledExceptionBehaviorLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Request: Unhandled Exception for Request {Name} {@Request}")]
    public static partial void LogUnhandledException(this ILogger logger, Exception exception, string name, object request);
}
