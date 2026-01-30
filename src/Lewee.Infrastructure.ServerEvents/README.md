# Lewee.Infrastructure.ServerEvents

Infrastructure utilities for [ASP.NET Core Web API Server-Sent Events](https://www.milanjovanovic.tech/blog/server-sent-events-in-aspnetcore-and-dotnet-10).

## Purpose

This package provides the **server-side** infrastructure for broadcasting client events to connected Blazor clients using Server-Sent Events (SSE). It includes the client event broadcaster, channel configuration, and SSE endpoint.

For **client-side** event reception in Blazor applications, see [Lewee.Infrastructure.Fluxor](../Lewee.Infrastructure.Fluxor/README.md).

## Dependencies

- `Microsoft.AspNetCore.App` - ASP.NET Core framework
- `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` - Health check support
- `Lewee.Application` - Application layer with ClientEvent notification
- `Lewee.Common` - Common utilities

## Components

### Interfaces

#### IClientEventBroadcaster

Interface for broadcasting client events to subscribers:

```csharp
public interface IClientEventBroadcaster
{
    event EventHandler<ClientEventArgs>? OnClientEvent;
    void Broadcast(ClientEvent clientEvent);
}
```

#### ClientEventArgs

Event arguments containing the client event:

```csharp
public class ClientEventArgs : EventArgs
{
    public ClientEvent ClientEvent { get; }
}
```

### Server-Side Broadcasting

#### ClientEventBroadcaster

Implementation using .NET events for broadcasting client events to all connected subscribers.

#### ClientEventBroadcasterHandler

MediatR notification handler that broadcasts `ClientEvent` notifications published via `IMediator.Publish`.

### SSE Endpoint Configuration

#### AddClientEventChannel

Configures the client event channel and broadcaster:

```csharp
builder.Services.AddClientEventChannel();
```

#### MapSseEndpoint

Maps the SSE endpoint for client connections:

```csharp
app.MapSseEndpoint();
```

## Usage

### Server Setup

```csharp
// In Web API Program.cs
builder.Services.AddClientEventChannel();

var app = builder.Build();
app.MapSseEndpoint();
```

### Publishing Events

```csharp
// In command handler or any service
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CommandResult>
{
    private readonly IMediator mediator;

    public async Task<CommandResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // ... create order logic ...

        // Broadcast event to all connected clients
        await mediator.Publish(new ClientEvent
        {
            MessageType = "OrderCreated",
            TenantId = order.TenantId,
            Data = JsonSerializer.Serialize(orderDto)
        }, cancellationToken);

        return CommandResult.Success();
    }
}
```

For receiving events in Blazor, see [Lewee.Infrastructure.Fluxor](../Lewee.Infrastructure.Fluxor/README.md).
