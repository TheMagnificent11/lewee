# Lewee.Infrastructure.AspNet

This package is used configure infrastructure that is available in ASP.Net.

Specifically, it assists with configuring handlers to send specific events/messages via `SignalR`, and correlation ID middleware.

## Dependencies

- `Microsoft.AspNetCore.App` (framework reference, not package reference)
- [Correlate.AspNetCore](https://github.com/skwasjer/Correlate) (for correlation ID functionality)
- [Microsoft.Azure.SignalR.Management](https://learn.microsoft.com/en-us/azure/azure-signalr/) (for Azure SignalR integration)
- [Lewee.Application](../Lewee.Application/README.md)

## Configuration

In the code below, `services` in the code below is `Microsoft.Extensions.DependencyInjection.ServicesCollection` and `app` is a `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder` (`Microsoft.AspNetCore.Builder.WebApplication` implements `IEndpointRouteBuilder`).

### Client Events SignalR Configuration

```cs
services.ConfigureSignalR();
```

```cs
app.MapHub<ClientEventHub>("/events");
```

### Correlation ID Configuration

Configure correlation ID logging to track requests across your application:

```cs
services.AddCorrelationIdServices();
```

```cs
app.UseCorrelationIdMiddleware();
```

## Usage

### Client Events

Publishing a [ClientEvent](../Lewee.Application/Mediation/Notifications/ClientEvent.cs) via `FreeMediator` will ensure that it is handled by the by the [ClientEventHandler](./SignalR/ClientEventHandler.cs), which will send it to the appropriate `SignalR` clients (all if unauthenticated and the ones belonging to the authenticated user if authenticated).

*Note: The current Pizzeria sample application does not yet include ClientEvent examples. This functionality is available in the framework but not yet implemented in the sample application.*

### Correlation ID Logging

The `AddCorrelationIdServices` method configures correlation ID middleware using the [Correlate](https://github.com/skwasjer/Correlate) library. This enables request correlation tracking throughout your application.

**Configuration:**

- Uses the `X-Correlation-ID` header (defined in [Lewee.Common.RequestHeaders](../Lewee.Common/README.md#constants))
- Automatically generates correlation IDs for requests that don't include the header
- Makes correlation IDs available via `ICorrelationContextAccessor` for logging and tracing

**Usage in logging:**

```cs
// The correlation ID will be automatically included in log scopes
// when using structured logging with correlation-aware loggers
logger.LogInformation("Processing order for customer {CustomerId}", customerId);
```

The correlation ID flows through your entire request pipeline and can be used with structured logging to trace requests across services. See the [Pizzeria Store API](../../sample/Pizzeria.Store.Api/Program.cs) for a complete implementation example.
