using Microsoft.Extensions.Logging;

namespace Pizzeria.Store.Web.States;

internal static partial class MessageToActionMapperLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Received null message with CorrelationId={CorrelationId}")]
    public static partial void LogReceivedNullMessage(this ILogger logger, Guid correlationId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Mapping message: Type={MessageType}, CorrelationId={CorrelationId}")]
    public static partial void LogMappingMessage(this ILogger logger, string? messageType, Guid correlationId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully mapped {MessageType} to {ActionType}")]
    public static partial void LogSuccessfullyMapped(this ILogger logger, string messagetype, string actiontype);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No mapping found for message type {MessageType}")]
    public static partial void LogNoMappingFound(this ILogger logger, string? messagetype);
}
