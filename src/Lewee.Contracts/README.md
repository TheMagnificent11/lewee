# Lewee.Contracts

Shared contracts for client-server communication and Fluxor state management in Lewee framework applications.

## Purpose

This package provides the foundational contracts used for:

- Message passing between servers and Blazor clients via SignalR
- Interfaces for Fluxor state management actions

## Dependencies

- `System.Threading.Channels` - For the `ClientEventChannel` implementation

## Components

### Client Messaging

#### ClientMessage

The `ClientMessage` class is the primary data transfer object used to transport messages between server and client applications.

| Property | Type | Description |
|----------|------|-------------|
| `CorrelationId` | `Guid` | Tracks the message through the system for logging and debugging |
| `ContractAssemblyName` | `string` | Assembly name containing the JSON contract class for deserialization |
| `ContractFullClassName` | `string` | Full class name (including namespace) of the JSON contract class |
| `MessageJson` | `string` | Serialized JSON representation of the message payload |

#### ClientEventChannel

Thread-safe unbounded channel for passing client events to Blazor circuits. Uses `System.Threading.Channels` to enable multiple readers and writers.

```csharp
// Singleton channel for all circuits
services.AddSingleton<ClientEventChannel>();

// Write events from SignalR handler
await channel.Writer.WriteAsync(clientMessage);

// Read events in Blazor circuit
await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
{
    // Process message
}
```

### State Management Interfaces

These interfaces define contracts for Fluxor actions used in client-side state management:

| Interface | Description |
|-----------|-------------|
| `IRequestAction` | Base interface for actions that initiate a request, containing a `CorrelationId` |
| `IRequestSuccessAction` | Interface for actions indicating successful request completion with `CorrelationId` |
| `IQuerySuccessAction<T>` | Generic interface for successful query actions carrying `Data` of type `T` |
| `IRequestErrorAction` | Interface for failed request actions with `CorrelationId` and `ErrorMessage` |
| `IMessageReceivedAction` | Interface for actions dispatched when a server message is received |

## Usage

### Implementing State Management Actions

```csharp
// Request action to initiate a query
public record GetOrdersAction(Guid CorrelationId) : IRequestAction;

// Success action with query data
public record GetOrdersSuccessAction(Guid CorrelationId, OrderDto[] Data)
    : IQuerySuccessAction<OrderDto[]>;

// Error action for failures
public record GetOrdersErrorAction(Guid CorrelationId, string ErrorMessage)
    : IRequestErrorAction;
```

### Client-Side Message Deserialization

On the client side (in Blazor applications), `ClientMessage` is deserialized back to the original type:

```csharp
var assembly = Assembly.Load(clientMessage.ContractAssemblyName);
var targetType = assembly.GetType(clientMessage.ContractFullClassName);
var messageBody = JsonSerializer.Deserialize(clientMessage.MessageJson, targetType);
```

## Integration with Other Lewee Packages

| Package | Integration |
|---------|-------------|
| `Lewee.Application` | `ClientEvent` uses `ClientMessage` as transport format for domain event notifications |
| `Lewee.Infrastructure.AspNet` | `ClientEventHandler` sends `ClientMessage` objects via SignalR hubs |
| `Lewee.Blazor` | `MessageDeserializer` processes `ClientMessage` objects and maps them to Fluxor actions |
| `Lewee.StateManagement` | `ReducerExtensions` work with the state management interfaces for reducer patterns |
