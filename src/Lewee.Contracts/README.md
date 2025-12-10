# Lewee.Contracts

Shared contracts for client-server communication in Lewee framework applications.

## Purpose

This package provides the foundational contracts used for message passing between servers and Blazor clients via SignalR.

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
| `Lewee.StateManagement` | References this package and provides state management action interfaces |
