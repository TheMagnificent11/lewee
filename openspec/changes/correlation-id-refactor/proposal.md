## Why

Commands, queries, and Fluxor actions currently require a `CorrelationId` to be passed explicitly through every constructor (`IApplicationRequest.CorrelationId`, `IRequestAction.CorrelationId`), even though the `Correlate` package already exposes an ambient `ICorrelationContextAccessor` for the current request/HTTP call. This duplicates the correlation ID as both an explicit parameter and an ambient value, forces every command/query author to remember to thread it through, and couples the application layer to a concern that should be cross-cutting infrastructure. Removing the explicit parameter simplifies application-layer contracts and centralizes correlation ID resolution behind `ICorrelationContextAccessor`.

## What Changes

- **BREAKING**: Remove `CorrelationId` from `IApplicationRequest` (and therefore from `ICommand` / `IQuery<T>`); commands and queries no longer declare or accept a `CorrelationId` constructor parameter.
- `CorrelationIdLoggingBehavior` resolves the correlation ID by injecting `ICorrelationContextAccessor` instead of reading `request.CorrelationId`.
- FastEndpoints `CommandEndpoint<TRequest>` / `QueryEndpoint<T>` base classes stop passing `this.CorrelationId` into command/query constructors (the accessor already sets the ambient correlation ID from the incoming HTTP request via the `Correlate` middleware).
- Sample application commands/queries (e.g. `StartOrderCommand`, `AddPizzaToOrderCommand`, `CreateCustomerCommand`, `GetPizzasQuery`) drop the `CorrelationId` constructor parameter, and the endpoints that construct them are updated accordingly.
- Add extension method(s) on `ICorrelationContextAccessor` (in `Lewee.Infrastructure.Fluxor`) that the Fluxor `CommandEffects<>` / `QueryEffects<>` / `RequestEffects<>` base classes use to explicitly set the ambient correlation ID from the current `IRequestAction.CorrelationId` before dispatching downstream, so state-management effects consistently propagate the correlation ID onto `ICorrelationContextAccessor` rather than only using it for a logging scope.
- `DomainEvent` and other domain/application code that currently accepts an explicit `correlationId` argument are reviewed; those that only exist to plumb the value from a command are simplified to resolve it from `ICorrelationContextAccessor` where appropriate, keeping `DomainEvent` itself framework-agnostic (Domain layer must not depend on `Correlate`, so the correlation ID continues to be passed into domain events by the application layer, now sourced from the accessor instead of the command).

## Capabilities

### New Capabilities
- `correlation-id-propagation`: Defines how correlation IDs are resolved and propagated across HTTP requests, MediatR pipeline behaviors, and Fluxor state-management effects using `ICorrelationContextAccessor`, without requiring commands/queries/actions to declare an explicit `CorrelationId` parameter.

### Modified Capabilities
- None (no existing `openspec/specs/` capabilities are defined yet).

## Impact

- **Affected code**:
  - `src/Lewee.Application/Mediation/Requests/IApplicationRequest.cs`, `ICommand.cs`, `IQuery.cs`
  - `src/Lewee.Application/Mediation/Behaviors/CorrelationIdLoggingBehavior.cs`
  - `src/Lewee.Infrastructure.FastEndpoints/CorrelationContextAccessorExtensions.cs` and its `CommandEndpoint<T>` / `QueryEndpoint<T>` base classes
  - `src/Lewee.Infrastructure.Fluxor/Observability/CorrelationContextAccessorExtensions.cs`, `LoggingExtensions.cs`, and the `CommandEffects<>` / `QueryEffects<>` / `RequestEffects<>` base classes
  - Sample commands/queries in `sample/Pizzeria.Store.Application/**` and endpoints in `sample/Pizzeria.Store.Api/**` that currently pass `CorrelationId` into constructors
  - Corresponding unit tests in `tests/Lewee.Application.Tests.Unit/CorrelationIdLoggingBehaviorTests.cs` and any sample tests constructing commands/queries with a `CorrelationId` argument
- **Breaking change**: Any consumer of the `Lewee.Application` package constructing commands/queries with an explicit `CorrelationId` argument will need to remove it. This is called out as a framework (`src/`) breaking change per `decision-making.instructions.md` guidance on backward compatibility for framework packages.
- **Dependencies**: No new NuGet packages; continues to rely on the existing `Correlate` package already referenced by `Lewee.Infrastructure.Correlate` and `Lewee.Infrastructure.Fluxor`.
