using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

internal static partial class ClientEventReceiverLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = LogMessages.StartedListening)]
    public static partial void LogStartedListening(this ILogger logger, string? userId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = LogMessages.StoppedListening)]
    public static partial void LogStoppedListening(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = LogMessages.ProcessingClientEvent)]
    public static partial void LogProcessingClientEvent(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = LogMessages.CouldNotResolveType)]
    public static partial void LogCouldNotResolveType(this ILogger logger, string typeName);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = LogMessages.CouldNotDeserializeMessage)]
    public static partial void LogCouldNotDeserializeMessage(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = LogMessages.NoActionMapped)]
    public static partial void LogNoActionMapped(this ILogger logger, string? messageType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = LogMessages.DispatchedAction)]
    public static partial void LogDispatchedAction(this ILogger logger, string? actionType);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = LogMessages.ErrorDispatchingAction)]
    public static partial void LogErrorDispatchingAction(this ILogger logger, Exception exception, string? actionType);

    [LoggerMessage(
        EventId = 8,
        Level = LogLevel.Error,
        Message = LogMessages.SseConnectionError)]
    public static partial void LogSseConnectionError(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 9,
        Level = LogLevel.Warning,
        Message = LogMessages.SseDeserializationError)]
    public static partial void LogSseDeserializationError(this ILogger logger, Exception exception);

    internal static class LogMessages
    {
        public const string StartedListening = "Started listening for client events. UserId: {UserId}";
        public const string StoppedListening = "Stopped listening for client events";
        public const string ProcessingClientEvent = "Processing client event";
        public const string CouldNotResolveType = "Could not resolve type: {TypeName}";
        public const string CouldNotDeserializeMessage = "Could not deserialize message";
        public const string NoActionMapped = "No action mapped for message type: {MessageType}";
        public const string DispatchedAction = "Dispatched action: {ActionType}";
        public const string ErrorDispatchingAction = "Error dispatching action: {ActionType}";
        public const string SseConnectionError = "Error receiving SSE events, reconnecting...";
        public const string SseDeserializationError = "Failed to deserialize SSE event data";
    }
}
