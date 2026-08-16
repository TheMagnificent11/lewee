## 1. Application Layer Contracts (Domain → Application boundary)

- [x] 1.1 Remove `CorrelationId` from `IApplicationRequest` in `src/Lewee.Application/Mediation/Requests/IApplicationRequest.cs` (remove the interface entirely if it becomes empty, updating `ICommand`/`IQuery<T>` accordingly)
- [x] 1.2 Update `ICommand` and `IQuery<T>` in `src/Lewee.Application/Mediation/Requests/` to no longer expose `CorrelationId`
- [x] 1.3 Add a shared `ICorrelationContextAccessor` correlation ID resolution helper (parse `CorrelationContext.CorrelationId` to `Guid`, falling back to `Guid.NewGuid()`) reusable by `Lewee.Application` and `Lewee.Infrastructure.FastEndpoints`

## 2. MediatR Pipeline Behavior

- [x] 2.1 Update `CorrelationIdLoggingBehavior` in `src/Lewee.Application/Mediation/Behaviors/CorrelationIdLoggingBehavior.cs` to inject `ICorrelationContextAccessor` and resolve the correlation ID from it instead of from `request.CorrelationId`
- [x] 2.2 Add/verify DI registration so `ICorrelationContextAccessor` is available wherever `CorrelationIdLoggingBehavior` is resolved (confirm `Lewee.Application` has access to the `Correlate` abstraction, referencing `src/Lewee.Application/ApplicationConfiguration.cs`)
- [x] 2.3 Update `tests/Lewee.Application.Tests.Unit/CorrelationIdLoggingBehaviorTests.cs` to construct commands/queries without `CorrelationId` and inject a fake/mock `ICorrelationContextAccessor`, covering both a populated and an empty/missing correlation context

## 3. FastEndpoints Infrastructure

- [x] 3.1 Update `src/Lewee.Infrastructure.FastEndpoints/CorrelationContextAccessorExtensions.cs` and the `CommandEndpoint<TRequest>` / `QueryEndpoint<T>` base classes so they no longer need to pass `CorrelationId` into command/query constructors
- [x] 3.2 Update sample endpoints in `sample/Pizzeria.Store.Api/**` that construct commands/queries with `this.CorrelationId` (e.g. `StartOrderEndpoint.cs`, `AddPizzaToOrderEndpoint.cs`) to drop the argument

## 4. Fluxor State Management Infrastructure

- [x] 4.1 Add/rename an extension method on `ICorrelationContextAccessor` in `src/Lewee.Infrastructure.Fluxor/Observability/CorrelationContextAccessorExtensions.cs` that explicitly sets the accessor's `CorrelationContext` from an `IRequestAction.CorrelationId`
- [x] 4.2 Update `CommandEffects<>`, `QueryEffects<>`, and `RequestEffects<>` base classes to call this extension method before executing the command/query (in addition to or as part of `BeginCorrelationIdScope`), so `ICorrelationContextAccessor` reflects the action's correlation ID for the duration of effect execution
- [x] 4.3 Verify `LoggingExtensions.BeginCorrelationIdScope` still functions correctly alongside the explicit accessor assignment (no duplicate work, no regressions in logging scope behavior)

## 5. Sample Application Updates

- [x] 5.1 Remove `CorrelationId` from sample command records (`StartOrderCommand`, `AddPizzaToOrderCommand`, `CreateCustomerCommand`, `GetPizzasQuery`, and any others found via search) in `sample/Pizzeria.Store.Application/**`
- [x] 5.2 Update any command/query handlers that previously used `request.CorrelationId` (e.g. when constructing a `DomainEvent`) to instead inject `ICorrelationContextAccessor` and resolve the correlation ID from it
- [x] 5.3 Update sample Fluxor actions/effects in `sample/Pizzeria.Store.StateManagement/**` if they reference `CorrelationId` in a way affected by the accessor changes

## 6. Test Updates and Verification

- [x] 6.1 Search the full repo for remaining `CorrelationId` constructor usages on commands/queries (`grep -r "CorrelationId" src/ sample/ tests/ sample-tests/`) and update any missed call sites
- [x] 6.2 Update or add unit tests in `tests/` covering the new `ICorrelationContextAccessor`-based extension methods for Fluxor effects
- [x] 6.3 Run `dotnet build --configuration Release --nologo` and confirm no errors or warnings
- [x] 6.4 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and confirm all unit tests pass
- [x] 6.5 Run `dotnet format` to confirm code style compliance
