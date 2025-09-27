# Lewee.Infrastructure.AspNet

This package is used configure infrastructure that is available in ASP.Net.

Specifically, it assist with configuring an [AuthenticatedUserService](./Auth/AuthenticatedUserService.cs) and a handler to send specific events/messages via `SignalR`.

## Dependencies

- `Microsoft.AspNetCore.App` (framework reference, not package reference)
- [Serilog](https://github.com/serilog/serilog)
- [Correlate](https://github.com/skwasjer/Correlate) (for correlation ID functionality)
- [Lewee.Application](../Lewee.Application/README.md)

## Configuration

In the code below, `services` in the code below is `Microsoft.Extensions.DependencyInjection.ServicesCollection` and `app` is a `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder` (`Microsoft.AspNetCore.Builder.WebApplication` implements `IEndpointRouteBuilder`).

### Authenticated User Configuration

```cs
services.ConfigureAuthenticatedUserService();
```

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

### Authenticated User

Inject `IAuthenticatedUserService` into an services that need to obtain the `UserId` of a user (this is the value stored as the [name identifier claim](http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier) in a JWT).

[Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md) uses this service to populate the created/modified by user ID on entity table records.

### Client Events

Publishing a [ClientEvent](../Lewee.Application/Mediation/Notifications/ClientEvent.cs) via `FreeMediator` will ensure that it is handled by the by the [ClientEventHandler](./SignalR/ClientEventHandler.cs), which will send it to the appropriate `SignalR` clients (all if unauthenticated and the ones belonging to the authenticated user if authenticated).

*Note: The current Pizzeria sample application does not yet include ClientEvent examples. This functionality is available in the framework but not yet implemented in the sample application.*

### Correlation ID Logging

The `AddCorrelationIdServices` method configures correlation ID middleware using the [Correlate](https://github.com/skwasjer/Correlate) library. This enables request correlation tracking throughout your application.

**Configuration:**
- Uses the `X-Correlation-ID` header (defined in [Lewee.Shared.RequestHeaders](../Lewee.Shared/README.md#constants))
- Automatically generates correlation IDs for requests that don't include the header
- Makes correlation IDs available via `ICorrelationContextAccessor` for logging and tracing

**Usage in logging:**
```cs
// The correlation ID will be automatically included in log scopes
// when using structured logging with correlation-aware loggers
logger.LogInformation("Processing order for customer {CustomerId}", customerId);
```

The correlation ID flows through your entire request pipeline and can be used with structured logging to trace requests across services. See the [Pizzeria Store API](../../sample/Pizzeria.Store.Api/Program.cs) for a complete implementation example.
