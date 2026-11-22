using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.ServerEvents;

internal static partial class ClientEventChannelHandlerLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No server-sent events channel found for user {UserId}")]
    public static partial void LogNoUserEventsChannelFound(this ILogger logger, string userId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Server-sent events channel was closed before the event could be written")]
    public static partial void LogChannelClosedOnWrite(this ILogger logger, Exception exception);
}
