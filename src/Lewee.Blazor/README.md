# Lewee.Blazor

This package provides comprehensive Blazor client-side infrastructure for building interactive web applications with state management, real-time messaging, and API integration.

## Dependencies

- [Fluxor](https://github.com/mrpmorris/Fluxor) (v6.8.0) - State management with Redux pattern
- [Microsoft.AspNetCore.SignalR.Client](https://learn.microsoft.com/en-us/aspnet/core/signalr) - Real-time web functionality
- [Correlate](https://github.com/skwasjer/Correlate) - Correlation ID functionality
- [Lewee.Common](../Lewee.Common/README.md) - Shared utilities and constants
- [Lewee.StateManagement](../Lewee.StateManagement/README.md) - State management base classes
- [Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md) - ASP.NET Core integration

## Features

- **State Management**: Fluxor-based Redux pattern implementation for Blazor
- **SignalR Integration**: Real-time messaging from server to client
- **Correlation ID Handling**: Automatic correlation ID propagation across HTTP requests
- **API Error Handling**: Structured exception handling for API calls
- **Service Discovery Support**: Integration with .NET Aspire service discovery

## Configuration

In the code below, `builder` is a `WebApplicationBuilder` and `services` is `Microsoft.Extensions.DependencyInjection.IServiceCollection`.

### Basic Configuration (Direct URI)

```cs
using Lewee.Blazor;

// Configure Lewee.Blazor with direct server address
services.AddLeweeBlazor<YourMessageToActionMapper>(
    serverBaseAddress: new Uri("https://localhost:5001"),
    useReduxDevTools: builder.Environment.IsDevelopment());
```

### Service Discovery Configuration (Recommended with .NET Aspire)

```cs
using Lewee.Blazor;

// Configure HTTP client for service discovery
const string ApiClientName = "MyApi";
services
    .AddHttpClient(ApiClientName, c => c.BaseAddress = new Uri("https://my-api-service"))
    .AddCorrelationIdDelegationHandler();

// Configure Lewee.Blazor with service discovery
services.AddLeweeBlazor<YourMessageToActionMapper>(
    httpClientName: ApiClientName,
    useReduxDevTools: builder.Environment.IsDevelopment());
```

### Configuration Options

- **TMapper**: Your implementation of `IMessageToActionMapper` that maps SignalR messages to Fluxor actions
- **serverBaseAddress**: Direct URI to your API server (when not using service discovery)
- **httpClientName**: Name of the HttpClient configured with service discovery (preferred for .NET Aspire)
- **useReduxDevTools**: Enable Redux DevTools browser extension for debugging (typically true in development)
- **httpMessageHandler**: Optional HTTP message handler for testing scenarios

## Usage

### State Management with Fluxor

Lewee.Blazor provides base classes for implementing the Fluxor state management pattern.

#### 1. Define Your State

```cs
using Lewee.Blazor.Fluxor;

public record OrderState : RequestState
{
    public Order? CurrentOrder { get; init; }
}
```

For query operations, use `QueryState<T>`:

```cs
public record PizzaListState : QueryState<IEnumerable<Pizza>>
{
    // Data property is inherited from QueryState<T>
}
```

#### 2. Define Actions

```cs
using Lewee.Blazor.Fluxor.Actions;

// Request action
public record StartOrderAction(Guid CorrelationId) : IRequestAction;

// Success action
public record StartOrderSuccessAction(Order Order, Guid CorrelationId) : IRequestSuccessAction;

// Error action
public record rrorAction(string ErrorMessage, Guid CorrelationId) : IRequestErrorAction;
```

#### 3. Create Reducers

```cs
using Fluxor;

public static class OrderReducers
{
    [ReducerMethod]
    public static OrderState OnStartOrder(OrderState state, StartOrderAction action)
        => state with { CorrelationId = action.CorrelationId, ErrorMessage = null };

    [ReducerMethod]
    public static OrderState OnStartOrderSuccess(OrderState state, StartOrderSuccessAction action)
        => state with { CurrentOrder = action.Order, CorrelationId = action.CorrelationId };

    [ReducerMethod]
    public static OrderState OnStartOrderError(OrderState state, StartOrderErrorAction action)
        => state with { ErrorMessage = action.ErrorMessage, CorrelationId = action.CorrelationId };
}
```

#### 4. Create Effects

```cs
using Lewee.Blazor.Fluxor;
using Fluxor;
using Correlate;
using Microsoft.Extensions.Logging;

public class OrderEffects : RequestEffects<OrderState, StartOrderAction, StartOrderSuccessAction, StartOrderErrorAction>
{
    private readonly IMyApiClient apiClient;

    public OrderEffects(
        IState<OrderState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<OrderEffects> logger,
        IMyApiClient apiClient)
        : base(state, correlationContextAccessor, logger)
    {
        this.apiClient = apiClient;
    }

    protected override async Task ExecuteRequestAsync(StartOrderAction action, IDispatcher dispatcher)
    {
        try
        {
            var order = await this.apiClient.StartOrderAsync();
            dispatcher.Dispatch(new StartOrderSuccessAction(order, action.CorrelationId));
        }
        catch (ApiException ex)
        {
            var errorMessage = ex.GetErrorMessage();
            dispatcher.Dispatch(new StartOrderErrorAction(errorMessage, action.CorrelationId));
        }
    }
}
```

#### 5. Use in Components

```razor
@using Fluxor
@inherits FluxorComponent
@inject IState<OrderState> OrderState
@inject IDispatcher Dispatcher

<button @onclick="StartOrder">Start Order</button>

@if (OrderState.Value.CurrentOrder != null)
{
    <p>Order ID: @OrderState.Value.CurrentOrder.Id</p>
}

@if (!string.IsNullOrEmpty(OrderState.Value.ErrorMessage))
{
    <p class="error">@OrderState.Value.ErrorMessage</p>
}

@code {
    private void StartOrder()
    {
        Dispatcher.Dispatch(new StartOrderAction(Guid.NewGuid()));
    }
}
```

### Real-time Messaging with SignalR

Lewee.Blazor automatically configures SignalR to receive messages from your server and dispatch them as Fluxor actions.

#### 1. Implement Message to Action Mapper

```cs
using Lewee.Blazor.Messaging;
using Lewee.Blazor.Fluxor.Actions;
using Microsoft.Extensions.Logging;

public class MessageToActionMapper : IMessageToActionMapper
{
    private readonly ILogger<MessageToActionMapper> logger;

    public MessageToActionMapper(ILogger<MessageToActionMapper> logger)
    {
        this.logger = logger;
    }

    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        this.logger.LogInformation("Mapping SignalR message: {MessageType}", message?.GetType().Name);

        return message switch
        {
            OrderDto order => new OrderCreatedAction(order, correlationId),
            OrderStatusDto status => new OrderStatusChangedAction(status, correlationId),
            _ => null
        };
    }
}
```

#### 2. Define Message-Received Actions

```cs
using Lewee.Blazor.Fluxor.Actions;

public record OrderCreatedAction(OrderDto Order, Guid CorrelationId) : IMessageReceivedAction;

public record OrderStatusChangedAction(OrderStatusDto Status, Guid CorrelationId) : IMessageReceivedAction;
```

#### 3. Create Reducers for Messages

```cs
[ReducerMethod]
public static OrderState OnOrderCreated(OrderState state, OrderCreatedAction action)
    => state with { CurrentOrder = action.Order, CorrelationId = action.CorrelationId };
```

#### 4. Server-Side Configuration

On your server, use [Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md) to configure SignalR and send messages:

```cs
// In Program.cs
services.ConfigureSignalR();
app.MapHub<ClientEventHub>("/events");

// To send a message to clients
await mediator.Publish(new ClientEvent(messageDto, userId));
```

### Correlation ID Handling

Lewee.Blazor automatically adds correlation IDs to all HTTP requests using the `CorrelationIdDelegatingHandler`.

```cs
// Manual configuration (if not using AddLeweeBlazor)
services
    .AddHttpClient<IMyApiClient>()
    .AddCorrelationIdDelegationHandler();
```

The correlation ID flows through:

1. Fluxor actions (via `IRequestAction.CorrelationId`)
2. HTTP requests (via `X-Correlation-ID` header)
3. Server-side logging (via [Lewee.Application](../Lewee.Application/README.md))
4. SignalR messages back to client
5. Client-side logging

### API Error Handling

Use the `ApiException` extension methods to extract user-friendly error messages:

```cs
using Lewee.Blazor.ErrorHandling;

try
{
    await apiClient.CreateOrderAsync(request);
}
catch (ApiException ex)
{
    // Automatically extracts error message from API response
    var errorMessage = ex.GetErrorMessage();

    // Log with structured logging
    this.logger.LogApiException(ex, correlationId);

    dispatcher.Dispatch(new CreateOrderErrorAction(errorMessage, correlationId));
}
```

## Sample Application

The [Pizzeria Store WebClient](../../sample/Pizzeria.Store.WebClient/) demonstrates a complete Lewee.Blazor implementation:

- **Program.cs**: Shows configuration with .NET Aspire service discovery
- **States/MessageToActionMapper.cs**: Example message-to-action mapping
- **States/Orders/**: Complete example of state, actions, reducers, and effects

### Running the Sample

```bash
cd sample
dotnet run --project Pizzeria.AppHost/
```

Navigate to the Aspire dashboard and access the Web Client to see Lewee.Blazor in action.

## Integration with Other Lewee Packages

This package integrates seamlessly with other Lewee packages:

- **[Lewee.Application](../Lewee.Application/README.md)**: Server-side CQRS handlers send `ClientEvent` notifications via SignalR
- **[Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md)**: Provides SignalR hub (`ClientEventHub`) for server-to-client messaging
- **[Lewee.Common](../Lewee.Common/README.md)**: Shared DTOs for API requests/responses, SignalR messages, and common constants
- **[Lewee.StateManagement](../Lewee.StateManagement/README.md)**: Base state classes and reducer extensions for Fluxor

## Components

**[Configuration.cs](./Configuration.cs)**: Main entry point with `AddLeweeBlazor` extension methods

### Messaging

- **[IMessageToActionMapper.cs](./Messaging/IMessageToActionMapper.cs)**: Interface for mapping SignalR messages to Fluxor actions
- **[MessageReceiverConfiguration.cs](./Messaging/MessageReceiverConfiguration.cs)**: SignalR connection configuration
- **[MessageDeserializer.cs](./Messaging/MessageDeserializer.cs)**: Deserializes `ClientMessage` objects to typed messages
- **[BlazorServerMessageReceiver.cs](./Messaging/BlazorServerMessageReceiver.cs)**: Blazor component for receiving SignalR messages
- **[Health/](./Messaging/Health/)**: Server health monitoring with Fluxor state

### HTTP

- **[CorrelationIdDelegatingHandler.cs](./Http/CorrelationIdDelegatingHandler.cs)**: Adds correlation ID to all HTTP requests

### Error Handling

- **[ApiException.cs](./ErrorHandling/ApiException.cs)**: Exception class for API errors (NSwag-compatible)
- **[ApiExceptionExtensions.cs](./ErrorHandling/ApiExceptionExtensions.cs)**: Helper methods to extract error messages

### State Management (via Lewee.StateManagement)

This package references [Lewee.StateManagement](../Lewee.StateManagement/README.md) which provides:

- **RequestState**: Base state class with correlation ID and error handling
- **QueryState\<T>**: Base state for query operations with data property
- **RequestEffects**: Base effects class for API calls
- **ReducerExtensions**: Helper extensions for reducers

## Best Practices

1. **Always use correlation IDs**: Pass `Guid.NewGuid()` when dispatching request actions for distributed tracing
2. **Enable Redux DevTools in development**: Set `useReduxDevTools: true` to debug state changes
3. **Implement proper error handling**: Use `try-catch` with `ApiException` in effects
4. **Use service discovery with .NET Aspire**: Preferred over hardcoded URIs for production applications
5. **Keep state immutable**: Always use `record` types with `init` properties
6. **Map all expected messages**: Return `null` from `IMessageToActionMapper.Map` for unrecognized message types
7. **Inherit from FluxorComponent**: Use `@inherits FluxorComponent` in Razor components for automatic re-rendering

## Troubleshooting

### SignalR Connection Issues

Check the browser console for SignalR connection errors. Ensure:

- The server has `ClientEventHub` mapped at `/events`
- CORS is properly configured on the server
- The base URL is correct

### Redux DevTools Not Working

1. Install the [Redux DevTools browser extension](https://github.com/reduxjs/redux-devtools)
2. Ensure `useReduxDevTools: true` is passed to `AddLeweeBlazor`
3. Open browser DevTools and select the Redux tab

### State Not Updating

1. Verify your component inherits from `FluxorComponent`
2. Check that reducers are returning new state instances (not mutating)
3. Ensure actions are being dispatched and reducers are being called

### Correlation IDs Missing

1. Verify `CorrelationIdDelegatingHandler` is configured on your HTTP client
2. Check that your actions implement `IRequestAction` with `CorrelationId` property
3. Ensure the server echoes correlation IDs back in SignalR messages
