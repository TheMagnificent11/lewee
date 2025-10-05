using Microsoft.Extensions.Logging;

namespace Pizzeria.Store.Application.Orders;

internal static partial class OrderingDomainEventHandlerLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Handling OrderStartedEvent for order {OrderId}")]
    public static partial void LogHandlingOrderStartedEvent(this ILogger logger, Guid orderId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Order {OrderId} not found when handling OrderStartedEvent - this indicates a critical system error")]
    public static partial void LogOrderNotFound(this ILogger logger, Guid orderId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Published OrderDto to SignalR for order {OrderId}")]
    public static partial void LogPublishedOrderDto(this ILogger logger, Guid orderId);
}
