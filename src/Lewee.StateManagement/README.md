# Lewee.StateManagement

Fluxor state management configuration and base classes for Blazor applications implementing the Redux pattern.

## Purpose

This package provides the foundational state management infrastructure for Blazor applications using [Fluxor](https://github.com/mrpmorris/Fluxor). It includes base state classes, reducer extension methods, effect base classes, and logging utilities for consistent state management patterns.

## Dependencies

- `Fluxor.Blazor.Web.ReduxDevTools` - Fluxor with Redux DevTools support
- `Correlate.DependencyInjection` - Correlation ID support
- `Lewee.Contracts` - State management action interfaces
- `Lewee.Shared` - Logging constants

## Components

### State Classes

#### IRequestState

Interface defining the base properties for request-based state:

| Property | Type | Description |
|----------|------|-------------|
| `CorrelationId` | `Guid` | Request correlation ID for tracing |
| `ErrorMessage` | `string?` | Error message from failed requests |

#### RequestState

Abstract base record for request state, implementing `IRequestState`:

```cs
public record OrderState : RequestState
{
    public Order? CurrentOrder { get; init; }
}
```

#### QueryState\<T>

Abstract base record for query state with a `Data` property for storing query results:

```cs
public record PizzaListState : QueryState<IEnumerable<PizzaDto>>
{
    // Data property is inherited from QueryState<T>
}
```

### Reducer Extensions

#### ReducerExtensions

Static class providing extension methods for common reducer patterns:

| Method | Description |
|--------|-------------|
| `OnRequest<TState, TAction>` | Sets correlation ID and clears error message |
| `OnQuery<TState, TStateData, TAction>` | Sets correlation ID, clears error and data |
| `OnQuerySuccess<TState, TStateData, TAction>` | Sets data from successful query action |
| `OnRequestError<TState, TAction>` | Sets error message from failed request |

**Usage:**

```cs
[ReducerMethod]
public static MyState OnRequest(MyState state, MyRequestAction action)
    => state.OnRequest(action);

[ReducerMethod]
public static MyState OnQuerySuccess(MyState state, MyQuerySuccessAction action)
    => state.OnQuerySuccess<MyState, MyDataDto, MyQuerySuccessAction>(action);

[ReducerMethod]
public static MyState OnError(MyState state, MyErrorAction action)
    => state.OnRequestError(action);
```

### Effects

#### RequestEffects\<TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction>

Abstract base class for implementing Fluxor effects that handle async operations like API calls:

**Type Parameters:**

- `TState` - State type extending `RequestState`
- `TRequestAction` - Action type implementing `IRequestAction`
- `TRequestSuccessAction` - Action type implementing `IRequestSuccessAction`
- `TRequestErrorAction` - Action type implementing `IRequestErrorAction`

**Features:**

- Automatically sets correlation context for distributed tracing
- Provides logging scope with correlation ID
- Handles success and error logging
- Abstract `ExecuteRequestAsync` method for implementing actual API calls

**Usage:**

```cs
public class GetPizzasEffects : RequestEffects<PizzaListState, GetPizzasAction, GetPizzasSuccessAction, GetPizzasErrorAction>
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

    protected override async Task ExecuteRequestAsync(GetPizzasAction action, IDispatcher dispatcher)
    {
        try
        {
            var pizzas = await this.client.GetPizzasAsync();
            dispatcher.Dispatch(new GetPizzasSuccessAction(action.CorrelationId, pizzas));
        }
        catch (ApiException ex)
        {
            dispatcher.Dispatch(new GetPizzasErrorAction(action.CorrelationId, ex.GetErrorMessage()));
        }
    }
}
```

### Logging

#### LoggingExtensions

Provides a `BeginCorrelationIdScope` extension method for `ILogger` to include correlation ID in log scopes:

```cs
using (logger.BeginCorrelationIdScope(correlationId))
{
    logger.LogInformation("Processing request...");
}
```

## Configuration

```cs
using Lewee.StateManagement;

// Configure Fluxor with state management assemblies
services.AddLeweeFluxor(
    useReduxDevTools: builder.Environment.IsDevelopment(),
    typeof(MyStateFeature).Assembly);
```

The `AddLeweeFluxor` method:

- Scans the entry assembly and provided assemblies for Fluxor features
- Optionally enables Redux DevTools for debugging
- Configures correlation context support

## Integration with Other Lewee Packages

| Package | Integration |
|---------|-------------|
| `Lewee.Contracts` | Uses `IRequestAction`, `IRequestSuccessAction`, `IRequestErrorAction`, `IQuerySuccessAction<T>` |
| `Lewee.Shared` | Uses `LoggingConsts` for consistent logging property names |
| `Lewee.Blazor` | References this package for state management configuration |

## Sample Application

See the [Pizzeria Store Web](../../sample/Pizzeria.Store.Web/) project for a complete implementation example using Fluxor state management.
