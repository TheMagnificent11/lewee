using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.ServerEvents;

internal static partial class ClientEventChannelHandlerLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No server-sent events channel found for user {UserId}")]
    public static partial void LogNoUserEventsChannelFound(this ILogger logger, string userId);
}
