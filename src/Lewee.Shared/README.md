# Lewee.Shared

This package provides shared utilities, constants, and extension methods that are commonly used across Lewee applications and other Lewee packages.

## Purpose

`Lewee.Shared` contains cross-cutting concerns and common functionality that is used throughout the Lewee framework ecosystem. It provides standardized constants for logging, HTTP headers, and useful extension methods for common operations.

## Dependencies

This package has minimal dependencies and only relies on the .NET Base Class Library (BCL).

## Components

### Constants

#### RequestHeaders
Contains standardized HTTP request header names used across Lewee applications:
- `CorrelationId` - The HTTP header name for correlation ID (`"X-Correlation-ID"`)

#### LoggingConsts
Provides consistent property names for structured logging:
- `CorrelationId` - Property name for correlation ID in logs (`"CorrelationId"`)
- `CorrelationIdHeaderKey` - Header key for correlation ID in logging context (`"correlationId"`)
- `TenantId` - Property name for tenant ID in multi-tenant applications (`"TenantId"`)
- `RequestType` - Property name for request type classification (`"RequestType"`)

#### HttpContextConsts
Contains constants for HTTP context items:
- `ClientId` - Key for storing SignalR client ID in HTTP context (`"SignalR-Client-Id"`)

### Extension Methods

#### EnumExtensions
Utility methods for working with enum values:
- `GetDescription(this Enum value)` - Gets the `DescriptionAttribute` description from an enum value if it exists, otherwise returns `ToString()` of the value
- `IsEquivalentToZero<TEnum>(this TEnum value)` - Determines whether the enum value is equivalent to zero

## Usage

### Working with Enum Descriptions

```cs
using Lewee.Shared;
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

### Checking Zero-Equivalent Enums

```cs
using Lewee.Shared;

public enum Priority
{
    None = 0,
    Low = 1,
    High = 2
}

// Usage
var priority = Priority.None;
bool isZero = priority.IsEquivalentToZero<Priority>(); // Returns true
```

### Using Standard Headers and Logging Constants

```cs
using Lewee.Shared;
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

// Multi-tenant Logging
public void LogWithTenant(ILogger logger, Guid tenantId, string requestType, string message)
{
    using (logger.BeginScope(new Dictionary<string, object>
    {
        [LoggingConsts.TenantId] = tenantId,
        [LoggingConsts.RequestType] = requestType
    }))
    {
        logger.LogInformation(message);
    }
}
```

### SignalR Client ID in HTTP Context

```cs
using Lewee.Shared;
using Microsoft.AspNetCore.Http;

public void StoreSignalRClientId(HttpContext context, string clientId)
{
    context.Items[HttpContextConsts.ClientId] = clientId;
}

public string? GetSignalRClientId(HttpContext context)
{
    return context.Items[HttpContextConsts.ClientId] as string;
}
```

## Integration with Other Lewee Packages

This package is referenced by other Lewee packages to ensure consistent:
- Correlation ID handling across HTTP requests and logging
- Multi-tenant logging patterns
- SignalR client identification
- Enum description handling

The constants defined here are used by:
- `Lewee.Application` for correlation ID and tenant logging behaviors
- `Lewee.Infrastructure.AspNet` for HTTP context management
- Other Lewee packages for consistent cross-cutting concerns
