using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

internal static partial class SseClientMessageReceiverLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = LogMessages.SseConnectionError)]
    public static partial void LogSseConnectionError(this ILogger logger, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = LogMessages.SseDeserializationError)]
    public static partial void LogSseDeserializationError(this ILogger logger, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = LogMessages.SseEventDataNull)]
    public static partial void LogSseEventDataNull(this ILogger logger);

    internal static class LogMessages
    {
        public const string SseConnectionError = "Error receiving SSE events, reconnecting...";
        public const string SseDeserializationError = "Failed to deserialize SSE event data";
        public const string SseEventDataNull = "SSE event received with null or empty data";
    }
}
