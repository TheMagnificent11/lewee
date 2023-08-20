# Lewee.Infrastructure.AspNet

This package is used configure infrastructure that is available in ASP.Net.

Specifically, it assist with configuring an [AuthenticatedUserService](./Auth/AuthenticatedUserService.cs) and a handler to send specific events/messages via `SignalR`.

## Dependencies

- `Microsoft.AspNetCore.App` (framework reference, not package reference)
- [Serilog](https://github.com/serilog/serilog)
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

## Usage

### Authenticated User

Inject `IAuthenticatedUserService` into an services that need to obtain the `UserId` of a user (this is the value stored as the [name identifier claim](http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier) in a JWT).

[Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md) uses this service to populate the created/modified by user ID on entity table records.

### Client Events

Publishing a [ClientEvent](../Lewee.Application/Mediation/Notifications/ClientEvent.cs) via `Mediatr` will ensure that it is handled by the by the [ClientEventHandler](./SignalR/ClientEventHandler.cs), which will send it to the appropriate `SignalR` clients (all if unauthenticated and the ones belonging to the authenticated user if authenticated).

[Sample code](../../sample/Sample.Restaurant.Application/TableDomainEventHandler.cs)
