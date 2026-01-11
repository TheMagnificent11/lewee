# Lewee.Blazor

This package provides comprehensive Blazor client-side infrastructure for building interactive web applications with state management and real-time messaging.

## Dependencies

- [Fluxor](https://github.com/mrpmorris/Fluxor) - State management with Redux pattern (via Lewee.StateManagement)
- [Microsoft.AspNetCore.SignalR.Client](https://learn.microsoft.com/en-us/aspnet/core/signalr) - Real-time web functionality
- [Correlate](https://github.com/skwasjer/Correlate) - Correlation ID functionality
- [Flurl](https://flurl.dev/) - URL building utilities
- [Lewee.Common](../Lewee.Common/README.md) - Shared utilities and constants
- [Lewee.StateManagement](../Lewee.StateManagement/README.md) - State management base classes
- [Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md) - ASP.NET Core integration

## Features

- **State Management**: Fluxor-based Redux pattern implementation for Blazor
- **SignalR Integration**: Real-time messaging from server to client via Azure SignalR
- **Server Health Monitoring**: Automatic health checks before establishing SignalR connections

## Configuration

In the code below, `services` is `Microsoft.Extensions.DependencyInjection.IServiceCollection`.

### Basic Configuration (Direct URI)

```cs
using Lewee.Blazor;

// Configure Lewee.Blazor with direct server address
services.AddLeweeBlazor<YourMessageToActionMapper>(
    serverBaseAddress: new Uri("https://localhost:5001"),
    useReduxDevTools: builder.Environment.IsDevelopment(),
    httpMessageHandler: null);
```

### Service Discovery Configuration (Recommended with .NET Aspire)

```cs
using Lewee.Blazor;

// Configure Lewee.Blazor with service discovery
services.AddLeweeBlazor<YourMessageToActionMapper>(
    apiAspireServiceName: "my-api-service",
    useReduxDevTools: builder.Environment.IsDevelopment());
```

### Configuration Options

| Parameter | Description |
|-----------|-------------|
| `TMapper` | Your implementation of `IMessageToActionMapper` that maps SignalR messages to Fluxor actions |
| `serverBaseAddress` | Direct URI to your API server (when not using service discovery) |
| `apiAspireServiceName` | Name of the Aspire service for service discovery (preferred for .NET Aspire) |
| `useReduxDevTools` | Enable Redux DevTools browser extension for debugging (typically true in development) |
| `httpMessageHandler` | Optional HTTP message handler for testing scenarios |

## Usage

### State Management with Fluxor

Lewee.Blazor uses [Lewee.StateManagement](../Lewee.StateManagement/README.md) which provides base classes for implementing the Fluxor state management pattern.

#### 1. Define Your State

```cs
using Lewee.StateManagement;

public record OrderState : RequestState<OrderDto>;
```

#### 2. Define Actions

```cs
using Lewee.StateManagement;

// Request action
public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}

// Success action
public record StartOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}

// Error action
public record StartOrderFailureAction : IRequestErrorAction
{
    public Guid CorrelationId { get; init; }
    public string ErrorMessage { get; init; }
}

// Message received action (from SignalR)
public record OrderCreatedAction : IMessageReceivedAction<OrderDto>
{
    public Guid CorrelationId { get; init; }
    public OrderDto Data { get; init; }
}
```

#### 3. Create Reducers

```cs
using Fluxor;

public static class OrderReducer
{
    [ReducerMethod]
    public static OrderState OnStartOrder(OrderState state, StartOrderAction action)
        => state with
        {
            IsSaving = true,
            CorrelationId = action.CorrelationId,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static OrderState OnStartOrderSuccess(OrderState state, StartOrderSuccessAction action)
        => state with
        {
            IsSaving = false,
            CorrelationId = action.CorrelationId,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static OrderState OnStartOrderFailure(OrderState state, StartOrderFailureAction action)
        => state with
        {
            IsSaving = false,
            CorrelationId = action.CorrelationId,
            ErrorMessage = action.ErrorMessage
        };

    [ReducerMethod]
    public static OrderState OnOrderCreated(OrderState state, OrderCreatedAction action)
        => state with
        {
            Data = action.Data,
            CorrelationId = action.CorrelationId
        };
}
```

#### 4. Create Effects

```cs
using Fluxor;
using Lewee.Infrastructure.Refit;

public class OrderEffects
{
    private readonly IMyApiClient apiClient;

    public OrderEffects(IMyApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    [EffectMethod]
    public async Task StartOrderAsync(StartOrderAction action, IDispatcher dispatcher)
    {
        try
        {
            await this.apiClient.StartOrderAsync();
            dispatcher.Dispatch(new StartOrderSuccessAction { CorrelationId = action.CorrelationId });
        }
        catch (ApiException ex)
        {
            dispatcher.Dispatch(new StartOrderFailureAction
            {
                CorrelationId = action.CorrelationId,
                ErrorMessage = ex.Message
            });
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

<button @onclick="StartOrder" disabled="@OrderState.Value.IsSaving">
    @if (OrderState.Value.IsSaving)
    {
        <span>Starting...</span>
    }
    else
    {
        <span>Start Order</span>
    }
</button>

@if (OrderState.Value.Data != null)
{
    <p>Order ID: @OrderState.Value.Data.Id</p>
}

@if (!string.IsNullOrEmpty(OrderState.Value.ErrorMessage))
{
    <p class="error">@OrderState.Value.ErrorMessage</p>
}

@code {
    private void StartOrder()
    {
        Dispatcher.Dispatch(new StartOrderAction());
    }
}
```

### Real-time Messaging with SignalR

Lewee.Blazor configures SignalR to receive messages from your server via Azure SignalR and dispatch them as Fluxor actions.

#### 1. Add the Message Receiver Component

In your main layout or App.razor:

```razor
@using Lewee.Blazor.Messaging

<MessageReceiverInitializer />
```

For Blazor Server applications using channels:

```razor
<BlazorServerMessageReceiver />
```

#### 2. Implement Message to Action Mapper

```cs
using Lewee.Blazor.Messaging;
using Lewee.StateManagement;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        return message switch
        {
            OrderDto order => new OrderCreatedAction
            {
                Data = order,
                CorrelationId = correlationId
            },
            _ => null
        };
    }
}
```

#### 3. Server-Side Configuration

On your server, use [Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md) to configure SignalR and send messages:

```cs
// In Program.cs
services.ConfigureSignalR();
app.MapHub<ClientEventHub>("/signalr");

// To send a message to clients
await mediator.Publish(new ClientEvent(correlationId, userId, messageDto));
```

### Server Health Monitoring

Lewee.Blazor automatically performs health checks before establishing SignalR connections:

1. On component initialization, a `HealthCheckAction` is dispatched
2. The `ServerHealthCheckEffects` calls the `/health` endpoint
3. On success, the SignalR `HubConnection` is started
4. On failure, retries up to 3 times with 3-second delays

## Components

### Configuration

**[Configuration.cs](./Configuration.cs)**: Main entry point with `AddLeweeBlazor` extension methods

### Messaging

| Component | Description |
|-----------|-------------|
| **[IMessageToActionMapper.cs](./Messaging/IMessageToActionMapper.cs)** | Interface for mapping SignalR messages to Fluxor actions |
| **[MessageReceiverConfiguration.cs](./Messaging/MessageReceiverConfiguration.cs)** | SignalR connection configuration |
| **[MessageDeserializer.cs](./Messaging/MessageDeserializer.cs)** | Deserializes `ClientMessage` objects to typed messages |
| **[MessageReceiverInitializer.cs](./Messaging/MessageReceiverInitializer.cs)** | Blazor component for Azure SignalR message receiving |
| **[BlazorServerMessageReceiver.cs](./Messaging/BlazorServerMessageReceiver.cs)** | Blazor Server component for channel-based message receiving |

### Health Monitoring

| Component | Description |
|-----------|-------------|
| **[ServerHealthState](./Messaging/Health/ServerHealthState.cs)** | State for server health tracking |
| **[HealthCheckService](./Messaging/Health/HealthCheckService.cs)** | HTTP client for health endpoint calls |
| **[ServerHealthCheckEffects](./Messaging/Health/ServerHealthCheckEffects.cs)** | Fluxor effects for health check workflow |
| **[ServerHealthReducer](./Messaging/Health/ServerHealthReducer.cs)** | Reducers for health state updates |
| **[Health/Actions/](./Messaging/Health/Actions/)** | Health check actions (HealthCheckAction, HealthCheckSuccessAction, HealthCheckFailedAction) |

### State Management (via Lewee.StateManagement)

This package references [Lewee.StateManagement](../Lewee.StateManagement/README.md) which provides:

- **RequestState\<T>**: Base state class with IsLoading, IsSaving, CorrelationId, Data, and ErrorMessage
- **IRequestAction**: Base interface for request actions with CorrelationId
- **IRequestSuccessAction**: Interface for success actions
- **IRequestErrorAction**: Interface for error actions with ErrorMessage
- **IQuerySuccessAction\<T>**: Interface for query success with Data
- **IMessageReceivedAction\<T>**: Interface for SignalR message actions with Data
- **ReducerExtensions**: Helper extensions for common reducer patterns

## Sample Application

The [Pizzeria Store Web](../../sample/Pizzeria.Store.Web/) demonstrates a complete Lewee.Blazor implementation:

- **Program.cs**: Shows configuration with .NET Aspire service discovery
- **MessageToActionMapper.cs**: Example message-to-action mapping
- **Pizzeria.Store.StateManagement/**: Complete state management with actions, reducers, and effects

### Running the Sample

```bash
cd sample
dotnet run --project Pizzeria.AppHost/
```

Navigate to the Aspire dashboard and access the Web application to see Lewee.Blazor in action.

## Integration with Other Lewee Packages

This package integrates seamlessly with other Lewee packages:

| Package | Integration |
|---------|-------------|
| **[Lewee.Application](../Lewee.Application/README.md)** | Server-side CQRS handlers send `ClientEvent` notifications via SignalR |
| **[Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md)** | Provides SignalR hub and `ClientEventChannel` for messaging |
| **[Lewee.Infrastructure.Refit](../Lewee.Infrastructure.Refit/README.md)** | HTTP client configuration with Refit, authentication, and correlation ID handling |
| **[Lewee.Common](../Lewee.Common/README.md)** | Shared `ClientMessage` DTO and `RequestHeaders` constants |
| **[Lewee.StateManagement](../Lewee.StateManagement/README.md)** | Base state classes, action interfaces, and Fluxor configuration |

## Best Practices

1. **Always use correlation IDs**: Let actions generate default `Guid.NewGuid()` for distributed tracing
2. **Enable Redux DevTools in development**: Set `useReduxDevTools: true` to debug state changes
3. **Use service discovery with .NET Aspire**: Preferred over hardcoded URIs for production applications
4. **Keep state immutable**: Always use `record` types with `init` properties
5. **Map all expected messages**: Return `null` from `IMessageToActionMapper.Map` for unrecognized message types
6. **Inherit from FluxorComponent**: Use `@inherits FluxorComponent` in Razor components for automatic re-rendering

## Troubleshooting

### SignalR Connection Issues

Check the browser console for SignalR connection errors. Ensure:

- The server has SignalR hub mapped at `/signalr`
- The `/health` endpoint returns a success status
- CORS is properly configured on the server
- The base URL is correct

### Health Check Failures

The system retries health checks up to 3 times. If all fail:

1. Check that your API server is running
2. Verify the `/health` endpoint is accessible
3. Check network connectivity and firewall rules

### Redux DevTools Not Working

1. Install the [Redux DevTools browser extension](https://github.com/reduxjs/redux-devtools)
2. Ensure `useReduxDevTools: true` is passed to `AddLeweeBlazor`
3. Open browser DevTools and select the Redux tab

### State Not Updating

1. Verify your component inherits from `FluxorComponent`
2. Check that reducers are returning new state instances (not mutating)
3. Ensure actions are being dispatched and reducers are being called

### Correlation IDs Missing

1. Verify `CorrelationIdDelegatingHandler` is configured (automatic with `AddApiClient`)
2. Check that your actions implement `IRequestAction` with `CorrelationId` property
3. Ensure the server echoes correlation IDs back in SignalR messages

### Authentication Token Missing

1. Ensure `AuthTokenDelegatingHandler` is configured (automatic with `AddApiClient`)
2. Verify the user is authenticated and has an `access_token`
3. Check HttpContext accessibility in your Blazor hosting model
