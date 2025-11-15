using Microsoft.Extensions.Logging;

namespace Pizzeria.Store.Application.Customers;

internal static partial class CustomerCreatedEventHandlerLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Handling CustomerCreatedEvent for customer {CustomerId}")]
    public static partial void LogHandlingCustomerCreatedEvent(this ILogger logger, Guid customerId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Published CustomerDto for customer {CustomerId}")]
    public static partial void LogPublishedCustomerDto(this ILogger logger, Guid customerId);
}
