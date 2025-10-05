using Microsoft.Extensions.Logging;

namespace Pizzeria.Store.Application.Orders;

internal static partial class StartOrderCommandLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Order {OrderId} started by user {UserId}")]
    public static partial void LogOrderStarted(this ILogger logger, Guid orderId, string userId);
}
