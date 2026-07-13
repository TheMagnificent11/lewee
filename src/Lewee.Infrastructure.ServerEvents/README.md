# Lewee.Infrastructure.ServerEvents

Infrastructure utilities for [ASP.NET Core Web API Server-Sent Events](https://www.milanjovanovic.tech/blog/server-sent-events-in-aspnetcore-and-dotnet-10).

## Purpose

This package provides the **server-side** infrastructure for broadcasting client events to connected Blazor clients using Server-Sent Events (SSE). It includes the client event broadcaster, channel configuration, and SSE endpoint.

For **client-side** event reception in Blazor applications, see [Lewee.Infrastructure.Fluxor](../Lewee.Infrastructure.Fluxor/README.md).

## Dependencies

- `Microsoft.AspNetCore.App` - ASP.NET Core framework
- `Lewee.Application` - Application layer with ClientEvent notification
- `Lewee.Common` - Common utilities

## Components

### ClientEventChannelHandler

MediatR notification handler that writes `ClientEvent` notifications to a channel for SSE broadcasting:

```csharp
internal sealed class ClientEventChannelHandler : INotificationHandler<ClientEvent>
{
    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        await this.channelWriter.WriteAsync(notification, cancellationToken);
    }
}
```

### SSE Endpoint Configuration

#### AddClientEventBroadcaster

Configures the client event channel and handler:

```csharp
builder.Services.AddClientEventBroadcaster();
```

This method:
- Creates an unbounded channel for `ClientEvent` messages
- Registers the channel reader and writer as singletons
- Registers `ClientEventChannelHandler` as a notification handler

#### MapSseEndpoint

Maps the SSE endpoint at `/events` for client connections:

```csharp
app.MapSseEndpoint();
```

The endpoint:
- Requires authorization
- Filters events by the authenticated user's ID
- Streams events as Server-Sent Events

## Usage

### Server Setup

```csharp
// In Web API Program.cs
builder.Services.AddClientEventBroadcaster();

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

        // Publish event to channel for broadcasting to connected clients
        await mediator.Publish(new ClientEvent
        {
            ContractFullClassName = "OrderCreated",
            UserId = order.UserId,
            Data = JsonSerializer.Serialize(orderDto)
        }, cancellationToken);

        return CommandResult.Success();
    }
}
```

For receiving events in Blazor, see [Lewee.Infrastructure.Fluxor](../Lewee.Infrastructure.Fluxor/README.md).
