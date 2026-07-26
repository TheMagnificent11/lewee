using Microsoft.Extensions.Logging;

namespace Pizzeria.Store.Application.Orders;

internal static partial class AddPizzaToOrderCommandLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Pizza {PizzaId} added to order")]
    public static partial void LogPizzaAddedToOrder(this ILogger logger, Guid pizzaId);
}
