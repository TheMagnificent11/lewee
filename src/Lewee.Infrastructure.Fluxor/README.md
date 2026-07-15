# Lewee.Infrastructure.Fluxor

Fluxor state management configuration and base classes for Blazor applications implementing the Redux pattern.

## Purpose

This package provides the foundational state management infrastructure for Blazor applications using [Fluxor](https://github.com/mrpmorris/Fluxor). It includes base state classes, reducer extension methods, effect base classes, action interfaces, and observability utilities.

## Dependencies

- `Fluxor.Blazor.Web.ReduxDevTools` - Fluxor with Redux DevTools support
- `Correlate.DependencyInjection` - Correlation ID support
- `Microsoft.Extensions.Http` - HttpClient factory support
- `Lewee.Common` - Shared contracts and utilities
- `Lewee.Infrastructure.Auth` - Authenticated user service

## Components

### State Management Interfaces

These interfaces define contracts for Fluxor actions used in client-side state management:

| Interface | Description |
|-----------|-------------|
| `IRequestAction` | Base interface for actions that initiate a request, containing a `CorrelationId` |
| `IRequestSuccessAction` | Interface for actions indicating successful request completion, extends `IRequestAction` |
| `IQuerySuccessAction<T>` | Generic interface for successful query actions carrying `Data` of type `T`, extends `IRequestSuccessAction` |
| `IRequestErrorAction` | Interface for failed request actions with `CorrelationId` and `ErrorMessage`, extends `IRequestAction` |
| `IMessageReceivedAction` | Interface for actions dispatched when a server message is received, extends `IRequestAction` |
| `IMessageReceivedAction<T>` | Generic interface for message received actions carrying `Data` of type `T`, extends `IMessageReceivedAction` |

**Usage:**

```csharp
// Request action to initiate a query
public record GetOrdersAction(Guid CorrelationId) : IRequestAction;

// Success action with query data
public record GetOrdersSuccessAction(Guid CorrelationId, OrderDto[] Data)
    : IQuerySuccessAction<OrderDto[]>;

// Error action for failures
public record GetOrdersErrorAction(Guid CorrelationId, string ErrorMessage)
    : IRequestErrorAction;

// Message received action with data
public record OrderUpdatedAction(Guid CorrelationId, OrderDto Data)
    : IMessageReceivedAction<OrderDto>;
```

### State Classes

#### IRequestState\<T>

Generic interface defining the base properties for request-based state:

| Property | Type | Description |
|----------|------|-------------|
| `IsLoading` | `bool` | Indicates whether the state is loading (query in progress) |
| `IsSaving` | `bool` | Indicates whether the state is saving (command in progress) |
| `CorrelationId` | `Guid` | Request correlation ID for tracing |
| `Data` | `T?` | The state data |
| `ErrorMessage` | `string?` | Error message from failed requests |

#### RequestState\<T>

Abstract base record for request state, implementing `IRequestState<T>`:

```cs
public record OrderState : RequestState<Order>
{
    // All properties are inherited from RequestState<T>
}

public record PizzaListState : RequestState<IEnumerable<PizzaDto>>
{
    // Data property stores the list of pizzas
}
```

### Reducer Extensions

#### ReducerExtensions

Static class providing extension methods for common reducer patterns:

| Method | Description |
|--------|-------------|
| `OnCommand<TState, TData, TAction>` | Sets correlation ID, sets `IsSaving` to true, clears error message, optionally clears data |
| `OnQuery<TState, TData, TAction>` | Sets correlation ID, sets `IsLoading` to true, clears data and error message |
| `OnCommandSuccess<TState, TData, TAction>` | Sets correlation ID, sets `IsSaving` to false, clears error message |
| `OnQuerySuccess<TState, TData, TAction>` | Sets correlation ID, sets `IsLoading` to false, sets data from action |
| `OnCommandError<TState, TData, TAction>` | Sets correlation ID, sets `IsSaving` to false, sets error message |
| `OnQueryError<TState, TData, TAction>` | Sets correlation ID, sets `IsLoading` to false, sets error message |
| `OnCommandCompleted<TState, TData, TAction>` | Sets correlation ID and data from message received action |

**Usage:**

```cs
[ReducerMethod]
public static MyState OnQuery(MyState state, MyQueryAction action)
    => state.OnQuery<MyState, MyDataDto, MyQueryAction>(action);

[ReducerMethod]
public static MyState OnCommand(MyState state, MyCommandAction action)
    => state.OnCommand<MyState, MyDataDto, MyCommandAction>(action, clearData: false);

[ReducerMethod]
public static MyState OnQuerySuccess(MyState state, MyQuerySuccessAction action)
    => state.OnQuerySuccess<MyState, MyDataDto, MyQuerySuccessAction>(action);

[ReducerMethod]
public static MyState OnCommandSuccess(MyState state, MyCommandSuccessAction action)
    => state.OnCommandSuccess<MyState, MyDataDto, MyCommandSuccessAction>(action);

[ReducerMethod]
public static MyState OnQueryError(MyState state, MyQueryErrorAction action)
    => state.OnQueryError<MyState, MyDataDto, MyQueryErrorAction>(action);

[ReducerMethod]
public static MyState OnCommandError(MyState state, MyCommandErrorAction action)
    => state.OnCommandError<MyState, MyDataDto, MyCommandErrorAction>(action);

[ReducerMethod]
public static MyState OnCommandCompleted(MyState state, MyMessageReceivedAction action)
    => state.OnCommandCompleted<MyState, MyDataDto, MyMessageReceivedAction>(action);
```

### Effects

The effects hierarchy provides base classes for implementing Fluxor effects that handle async operations.

#### RequestEffects\<TState, TData, TRequestSuccessAction, TRequestErrorAction>

Abstract base class for request effects that provides logging for success and error actions:

**Type Parameters:**

- `TState` - State type extending `RequestState<TData>`
- `TData` - Data type (must be a reference type)
- `TRequestSuccessAction` - Action type implementing `IRequestSuccessAction`
- `TRequestErrorAction` - Action type implementing `IRequestErrorAction`

**Features:**

- Provides logging scope with correlation ID for success and error effects
- Handles success and error logging automatically

#### QueryEffects\<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction>

Abstract base class for implementing Fluxor effects that handle query (read) operations:

**Type Parameters:**

- `TState` - State type extending `RequestState<TData>`
- `TData` - Data type (must be a reference type)
- `TRequestAction` - Action type implementing `IRequestAction`
- `TRequestSuccessAction` - Action type implementing `IQuerySuccessAction<TData>`
- `TRequestErrorAction` - Action type implementing `IRequestErrorAction`

**Features:**

- Automatically sets correlation context for distributed tracing
- Provides logging scope with correlation ID
- Handles success and error dispatching based on `QueryResult<TData>`
- Abstract `ExecuteQueryAsync` method for implementing actual API calls

**Usage:**

```cs
public class GetPizzasEffects : QueryEffects<PizzaListState, PizzaDto[], GetPizzasAction, GetPizzasSuccessAction, GetPizzasErrorAction>
{
    private readonly IPizzaApiClient client;

    public GetPizzasEffects(
        IState<PizzaListState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<GetPizzasEffects> logger,
        IPizzaApiClient client)
        : base(state, correlationContextAccessor, logger)
    {
        this.client = client;
    }

    protected override async Task<QueryResult<PizzaDto[]>> ExecuteQueryAsync(
        GetPizzasAction action,
        IDispatcher dispatcher)
    {
        var pizzas = await this.client.GetPizzasAsync();
        return QueryResult<PizzaDto[]>.Success(pizzas);
    }
}
```

#### CommandEffects\<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction, TMessageReceived>

Abstract base class for implementing Fluxor effects that handle command (write) operations with message-based completion:

**Type Parameters:**

- `TState` - State type extending `RequestState<TData>`
- `TData` - Data type (must be a reference type)
- `TRequestAction` - Action type implementing `IRequestAction`
- `TRequestSuccessAction` - Action type implementing `IRequestSuccessAction`
- `TRequestErrorAction` - Action type implementing `IRequestErrorAction`
- `TMessageReceived` - Action type implementing `IMessageReceivedAction<TData>`

**Features:**

- Automatically sets correlation context for distributed tracing
- Provides logging scope with correlation ID
- Handles success and error dispatching based on `CommandResult`
- Supports message-received pattern for server-side completion notifications
- Abstract `ExecuteCommandAsync` and `ExecuteCommandCompletedAsync` methods for implementing command logic

**Usage:**

```cs
public class CreateOrderEffects : CommandEffects<OrderState, Order, CreateOrderAction, CreateOrderSuccessAction, CreateOrderErrorAction, OrderCreatedAction>
{
    private readonly IOrderApiClient client;

    public CreateOrderEffects(
        IState<OrderState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<CreateOrderEffects> logger,
        IOrderApiClient client)
        : base(state, correlationContextAccessor, logger)
    {
        this.client = client;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        CreateOrderAction action,
        IDispatcher dispatcher)
    {
        await this.client.CreateOrderAsync(action.OrderData);
        return CommandResult.Success();
    }

    protected override Task ExecuteCommandCompletedAsync(
        OrderCreatedAction action,
        IDispatcher dispatcher)
    {
        // Handle any post-completion logic
        return Task.CompletedTask;
    }
}
```

### Observability

The `Lewee.Infrastructure.Fluxor.Observability` namespace provides utilities for logging and correlation context management.

#### LoggingExtensions

Provides a `BeginCorrelationIdScope` extension method for `ILogger` that sets the correlation context and includes the correlation ID in log scopes:

```cs
using Lewee.Infrastructure.Fluxor.Observability;

using (logger.BeginCorrelationIdScope(correlationContextAccessor, action))
{
    logger.LogInformation("Processing request...");
}
```

The method:
- Sets the correlation context on the `ICorrelationContextAccessor` from the action
- Creates a logging scope that includes the correlation ID

#### CorrelationContextAccessorExtensions

Provides extension methods for `ICorrelationContextAccessor` to manage correlation context:

| Method | Description |
|--------|-------------|
| `SetNewCorrelationId` | Sets the correlation ID on the correlation context from a request action |

```cs
using Lewee.Infrastructure.Fluxor.Observability;

// Set correlation context from a request action
correlationContextAccessor.SetNewCorrelationId(action);
```

## Configuration

```cs
using Lewee.Infrastructure.Fluxor;

// Configure Fluxor with state management assemblies
services.AddLeweeFluxor(
    useReduxDevTools: builder.Environment.IsDevelopment(),
    typeof(MyStateFeature).Assembly);
```

The `AddLeweeFluxor` method:

- Scans the entry assembly and provided assemblies for Fluxor features
- Optionally enables Redux DevTools for debugging
- Configures correlation context support via `AddCorrelate()`

### Server-Sent Events Configuration

```cs
using Lewee.Infrastructure.Fluxor;

// Register message to action mapper for SSE
services.AddSseMessageReceiver<MyMessageToActionMapper>();
```

The `AddSseMessageReceiver` method registers your custom `IMessageToActionMapper` implementation that maps server events to Fluxor actions.

## Client Event Receiver

The `ClientEventReceiver` component connects to the server's SSE endpoint via `HttpClient` and dispatches received events as Fluxor actions. This component should be placed in the application layout to receive events for the authenticated user.

### How It Works

1. The component uses `SseClientMessageReceiver` to establish an HTTP connection to the server's `/events` SSE endpoint
2. When a `ClientMessage` is received, it deserializes the message and uses `IMessageToActionMapper` to convert it to a Fluxor action
3. The action is dispatched to update the application state

### Usage

```razor
@using Lewee.Infrastructure.Fluxor

<ClientEventReceiver />
```

### IMessageToActionMapper

Implement this interface to map server messages to Fluxor actions:

```cs
public class MyMessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        return message switch
        {
            OrderDto order => new OrderCreatedAction
            {
                Data = order,
                CorrelationId = correlationId,
            },
            _ => null,
        };
    }
}
```

### SseClientMessageReceiver

The `SseClientMessageReceiver` class handles the HTTP-based SSE connection:

- Connects to the server's SSE endpoint using `HttpClient`
- Parses the SSE event stream and deserializes `ClientMessage` objects
- Raises `OnMessageReceived` events for each received message
- Implements automatic reconnection with retry logic on connection failures

## Integration with Other Lewee Packages

| Package | Integration |
|---------|-------------|
| `Lewee.Common` | Uses `LoggingConsts` for consistent logging, `ClientMessage` for SSE message contract, `IAuthenticatedUserService` for user context |
| `Lewee.Infrastructure.Auth` | Provides `IAuthenticatedUserService` implementation for authenticated user access |
| `Lewee.Infrastructure.ServerEvents` | Server-side SSE broadcasting (see [Lewee.Infrastructure.ServerEvents](../Lewee.Infrastructure.ServerEvents/README.md)) |

## Sample Application

See the [Pizzeria Store Web](../../sample/Pizzeria.Store.Web/) project for a complete implementation example using Fluxor state management and client event receiving.
