# Lewee.Contracts

This package provides message contract definitions used for client-server messaging throughout the Lewee framework, particularly for real-time communication via SignalR.

## Purpose

`Lewee.Contracts` defines the standardized message format and contracts used for sending structured messages from server-side domain events to client applications. This enables real-time updates and notifications in client applications when domain events occur on the server.

## Dependencies

This package has no external dependencies and only relies on the .NET Base Class Library (BCL).

## Components

### ClientMessage

The `ClientMessage` class is the primary data transfer object used to transport messages between server and client applications.

#### Properties

- **`CorrelationId`** (`Guid`) - Gets or sets the correlation ID that tracks the message through the system for logging and debugging purposes
- **`ContractAssemblyName`** (`string`) - Gets or sets the assembly name containing the JSON contract class for message deserialization
- **`ContractFullClassName`** (`string`) - Gets or sets the full class name (including namespace) of the JSON contract class for message deserialization  
- **`MessageJson`** (`string`) - Gets or sets the serialized JSON representation of the actual message payload

## Usage

### Basic Message Creation

The `ClientMessage` is typically created automatically by the framework when domain events are raised. Here's how it works conceptually:

```cs
// Domain event occurs on server
var domainEvent = new OrderCompletedEvent(orderId, customerId);

// Framework converts to ClientEvent (in Lewee.Application)
var clientEvent = new ClientEvent(correlationId, userId, domainEvent);

// ClientEvent is converted to ClientMessage for transport
var clientMessage = clientEvent.ToClientMessage();
// Results in:
// {
//   CorrelationId: correlationId,
//   ContractAssemblyName: "MyApp.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
//   ContractFullClassName: "MyApp.Contracts.OrderCompletedEvent",
//   MessageJson: "{\"OrderId\":\"123\",\"CustomerId\":\"456\"}"
// }
```

### Implementing Message Contracts

To create messages that can be sent to clients, implement the `IClientMessageContract` interface:

```cs
using Lewee.Contracts;

public class OrderStatusChangedEvent : IClientMessageContract
{
    public Guid OrderId { get; set; }
    public string Status { get; set; }
    public DateTime ChangedAt { get; set; }
}
```

### Client-Side Message Deserialization

On the client side (typically in Blazor applications), the `ClientMessage` is deserialized back to the original type:

```cs
// Framework deserializes using the contract information
var assembly = Assembly.Load(clientMessage.ContractAssemblyName);
var targetType = assembly.GetType(clientMessage.ContractFullClassName);
var messageBody = JsonSerializer.Deserialize(clientMessage.MessageJson, targetType);

// messageBody is now the original domain event object
```

## Integration with Other Lewee Packages

This package is used by several other Lewee packages:

- **`Lewee.Application`**: The `ClientEvent` class uses `ClientMessage` as its transport format when converting domain events for client notifications
- **`Lewee.Infrastructure.AspNet`**: The SignalR `ClientEventHandler` sends `ClientMessage` objects to connected clients via SignalR hubs
- **`Lewee.Blazor`**: The `MessageDeserializer` processes incoming `ClientMessage` objects and reconstructs the original message types for client-side handling

## Architecture Benefits

- **Type Safety**: The `IClientMessageContract` marker interface provides compile-time verification
- **Serialization Independence**: Message payload is pre-serialized as JSON, allowing for flexible transport mechanisms
- **Traceability**: Correlation IDs enable end-to-end tracking of messages through the system
- **Reflection-based Deserialization**: Contract assembly and class name information enables precise type reconstruction on the client
- **Decoupling**: Separates message transport concerns from the actual message content
