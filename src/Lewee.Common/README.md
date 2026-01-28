# Lewee.Common

This package provides shared utilities, constants, result types, and extension methods that are commonly used across Lewee applications and other Lewee packages.

## Purpose

`Lewee.Common` contains cross-cutting concerns and common functionality that is used throughout the Lewee framework ecosystem. It provides:

- **Result types** for command and query responses
- **Client message contracts** for server-to-client communication
- **Standardized constants** for logging and HTTP headers
- **Extension methods** for common operations

## Dependencies

- `FluentValidation` - For validation failure handling in result types

## Components

### Result Types

#### Result (Abstract Base)

Base class for all result types with success/failure status and validation errors:

| Property | Type | Description |
|----------|------|-------------|
| `IsSuccess` | `bool` | Whether the request was successfully processed |
| `Status` | `ResultStatus` | Enum indicating the type of result |
| `Errors` | `IList<ValidationFailure>` | Validation errors keyed by property |

#### ResultStatus

Enum for categorizing result outcomes:

| Value | Description |
|-------|-------------|
| `Success` | Request completed successfully |
| `ValidationError` | Input validation failed |
| `NotFound` | Requested resource not found |
| `DomainError` | Business rule violation |
| `UnexpectedError` | Unexpected system error |
| `NotApplicable` | Operation not applicable |

#### CommandResult

Result type for command operations (create, update, delete):

```cs
// Success
return CommandResult.Success();

// Failure with message
return CommandResult.Fail(ResultStatus.DomainError, "Order cannot be modified after completion");

// Failure with validation errors
return CommandResult.Fail(ResultStatus.ValidationError, validationFailures);
```

#### QueryResult\<T>

Generic result type for query operations with data payload:

```cs
// Success with data
return QueryResult<OrderDto>.Success(orderDto);

// Failure
return QueryResult<OrderDto>.Fail(ResultStatus.NotFound, "Order not found");
```

### Client Messaging

#### ClientMessage

Contract for server-to-client event messages:

| Property | Type | Description |
|----------|------|-------------|
| `CorrelationId` | `Guid` | Request correlation ID for tracing |
| `ContractAssemblyName` | `string` | Assembly containing the message type |
| `ContractFullClassName` | `string` | Full class name of the message type |
| `MessageJson` | `string` | Serialized message content |

### Constants

#### RequestHeaders

HTTP request header names used across Lewee applications:

- `CorrelationId` - The HTTP header name for correlation ID (`"X-Correlation-ID"`)

#### LoggingConsts

Property names for structured logging:

- `CorrelationId` - Property name for correlation ID in logs (`"CorrelationId"`)
- `CorrelationIdHeaderKey` - Header key for correlation ID in logging context (`"correlationId"`)
- `TenantId` - Property name for tenant ID in multi-tenant applications (`"TenantId"`)
- `RequestType` - Property name for request type classification (`"RequestType"`)

#### HttpContextConsts

Constants for HTTP context items:

- `ClientId` - Key for storing client ID in HTTP context (`"SignalR-Client-Id"`)

### Extension Methods

#### EnumExtensions

Utility methods for working with enum values:

- `GetDescription(this Enum value)` - Gets the `DescriptionAttribute` description from an enum value if it exists, otherwise returns `ToString()` of the value
- `IsEquivalentToZero<TEnum>(this TEnum value)` - Determines whether the enum value is equivalent to zero

## Usage

### Working with Result Types

```cs
using Lewee.Common;

// In a command handler
public async Task<CommandResult> Handle(CreateOrderCommand command, CancellationToken ct)
{
    if (!await this.repository.ExistsAsync(command.CustomerId, ct))
    {
        return CommandResult.Fail(ResultStatus.NotFound, "Customer not found");
    }

    var order = new Order(command.CustomerId);
    await this.repository.AddAsync(order, ct);

    return CommandResult.Success();
}

// In a query handler
public async Task<QueryResult<OrderDto>> Handle(GetOrderQuery query, CancellationToken ct)
{
    var order = await this.repository.RetrieveByIdAsync(query.OrderId, ct);

    if (order == null)
    {
        return QueryResult<OrderDto>.Fail(ResultStatus.NotFound, "Order not found");
    }

    return QueryResult<OrderDto>.Success(new OrderDto(order));
}
```

### Working with Enum Descriptions

```cs
using Lewee.Common;
using System.ComponentModel;

public enum OrderStatus
{
    [Description("Order Pending")]
    Pending = 0,

    [Description("Order Confirmed")]
    Confirmed = 1,

    Cancelled = 2
}

// Usage
var status = OrderStatus.Pending;
string description = status.GetDescription(); // Returns "Order Pending"

var cancelledStatus = OrderStatus.Cancelled;
string cancelledDescription = cancelledStatus.GetDescription(); // Returns "Cancelled" (ToString())
```

### Using Standard Headers and Logging Constants

```cs
using Lewee.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// HTTP Headers
public void AddCorrelationHeader(HttpContext context, string correlationId)
{
    context.Request.Headers[RequestHeaders.CorrelationId] = correlationId;
}

// Structured Logging
public void LogWithCorrelation(ILogger logger, string correlationId, string message)
{
    using (logger.BeginScope(new Dictionary<string, object>
    {
        [LoggingConsts.CorrelationId] = correlationId
    }))
    {
        logger.LogInformation(message);
    }
}
```

## Integration with Other Lewee Packages

This package is referenced by other Lewee packages to ensure consistent:

- Result type handling across commands and queries
- Client-server messaging via event broadcasting
- Correlation ID handling across HTTP requests and logging
- Multi-tenant logging patterns

The types and constants defined here are used by:

- `Lewee.Domain` - References for domain abstractions
- `Lewee.Application` - Uses result types for command/query responses
- `Lewee.Infrastructure.AspNet` - Uses constants for HTTP context management
- `Lewee.Infrastructure.Fluxor` - Uses constants for logging and client event handling
