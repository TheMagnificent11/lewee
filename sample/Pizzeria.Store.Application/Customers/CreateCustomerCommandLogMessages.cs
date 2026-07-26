using Microsoft.Extensions.Logging;

namespace Pizzeria.Store.Application.Customers;

internal static partial class CreateCustomerCommandLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Customer {CustomerId} created with external ID {ExternalId}")]
    public static partial void LogCustomerCreated(this ILogger logger, Guid customerId, string externalId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Customer {CustomerId} already exists with external ID {ExternalId}")]
    public static partial void LogCustomerAlreadyExists(this ILogger logger, Guid customerId, string externalId);
}
