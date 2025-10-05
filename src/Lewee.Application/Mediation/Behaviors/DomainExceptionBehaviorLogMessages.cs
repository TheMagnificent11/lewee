using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal static partial class DomainExceptionBehaviorLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Domain exception caught")]
    public static partial void LogDomainExceptionCaught(this ILogger logger, Exception exception);
}
