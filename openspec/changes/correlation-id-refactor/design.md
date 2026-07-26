## Context

Correlation IDs in Lewee are used to tie together logs across a single logical operation (an HTTP request, a MediatR command/query, or a Fluxor client-side action). Today the correlation ID is threaded two different ways:

1. **Explicit parameter**: `IApplicationRequest.CorrelationId` (inherited by `ICommand` and `IQuery<T>`) requires every command/query record to declare a `CorrelationId` constructor parameter. FastEndpoints endpoints populate it from `ICorrelationContextAccessor.GetCorrelationId()` (an extension method in `Lewee.Infrastructure.FastEndpoints`) before constructing the command/query.
2. **Ambient accessor**: The `Correlate` NuGet package (registered via `CorrelationIdConfiguration.AddCorrelationIdServices()`) already exposes `ICorrelationContextAccessor`, which holds the correlation ID for the current HTTP request (server-side) or is set manually before dispatching Fluxor actions (client-side, via `CorrelationContextAccessorExtensions.SetNewCorrelationId()`).

This duplication means the correlation ID is read from the accessor, copied onto a command/query/action, and then (in `CorrelationIdLoggingBehavior`) read back off the request instead of from the accessor directly. It also means every new command/query author must remember to add and thread the `CorrelationId` parameter, and framework consumers must pass it explicitly at every call site.

Constraints:
- `Lewee.Domain` must not take a dependency on `Correlate` or any infrastructure package (clean architecture boundary — Domain has no dependencies on other layers).
- `Lewee.Application` already depends on MediatR and can take a dependency on `Correlate`'s abstractions (`ICorrelationContextAccessor`) since `Lewee.Infrastructure.Correlate` currently sits at the infrastructure layer; `ICorrelationContextAccessor` itself is a lightweight interface from the `Correlate` package with no ASP.NET Core dependencies, so it is safe to consume from `Lewee.Application`.
- Fluxor effects run client-side (Blazor WebAssembly/Server) and do not have an HTTP request scope; the accessor there is populated manually, not by middleware.

## Goals / Non-Goals

**Goals:**
- Remove `CorrelationId` from `IApplicationRequest`/`ICommand`/`IQuery<T>` so commands and queries no longer need to declare or accept it.
- Resolve the correlation ID for MediatR pipeline logging via `ICorrelationContextAccessor` injected into `CorrelationIdLoggingBehavior`.
- Ensure FastEndpoints command/query endpoints continue to work without passing `CorrelationId` into constructors (the accessor is already populated by the `Correlate` middleware before the endpoint handler runs).
- Ensure Fluxor `CommandEffects<>` / `QueryEffects<>` / `RequestEffects<>` explicitly set the ambient correlation ID on `ICorrelationContextAccessor` from the current action before executing the command/query and dispatching follow-on actions, via a reusable extension method, so the accessor (not just the logging scope) reflects the active action's correlation ID for the duration of effect execution.
- Update all sample commands/queries/endpoints and existing unit tests to match the new contract.

**Non-Goals:**
- Changing how `Correlate` middleware extracts/generates correlation IDs from HTTP headers.
- Changing `DomainEvent`'s constructor contract (`DomainEvent` remains in `Lewee.Domain` and continues to accept an explicit `Guid correlationId` — it must not depend on `Correlate`). Application-layer code that constructs domain events will source that value from `ICorrelationContextAccessor` instead of from the command, but the domain event's own signature is unchanged.
- Introducing a new correlation ID abstraction to replace `Correlate` — `ICorrelationContextAccessor` remains the single source of truth.

## Decisions

1. **Remove `CorrelationId` from `IApplicationRequest`.**
   `IApplicationRequest` becomes a marker interface (or is removed entirely if it has no remaining members) and `ICommand`/`IQuery<T>` no longer expose a `CorrelationId` property.
   *Alternative considered*: Keep `CorrelationId` as an optional/nullable property defaulting to the accessor value. Rejected because it preserves the duplication this change intends to remove and still requires each record to declare the parameter.

2. **Inject `ICorrelationContextAccessor` directly into `CorrelationIdLoggingBehavior`.**
   The behavior's constructor takes `ICorrelationContextAccessor` and reads `accessor.CorrelationContext?.CorrelationId` (parsing to `Guid`, falling back to `Guid.NewGuid()` if absent, matching existing `GetCorrelationId()` extension behavior). Move/reuse the existing `GetCorrelationId()` extension method to a shared location (e.g. `Lewee.Application` or keep in `Lewee.Infrastructure.Correlate` and reference it from `Lewee.Application`) so both FastEndpoints and the MediatR behavior use the same parsing logic instead of duplicating it.
   *Alternative considered*: Have `CorrelationIdConfiguration` register a small wrapper service in `Lewee.Application` that exposes `Guid CorrelationId { get; }` computed from the accessor, to avoid a direct dependency on `Correlate` types in `Lewee.Application`. Rejected for this iteration to keep the change minimal — `Lewee.Application` already has an indirect expectation of correlation infrastructure being present, and `ICorrelationContextAccessor` has no ASP.NET Core dependencies.

3. **FastEndpoints base classes stop passing `CorrelationId` into commands/queries.**
   `CommandEndpoint<TRequest>` / `QueryEndpoint<T>` no longer need the `CorrelationId` property once command/query constructors no longer accept it. The `GetCorrelationId()` extension method can remain for any endpoint that still needs to log or return the correlation ID explicitly (e.g., in a response header), but it is no longer required for constructing requests.

4. **Add a `SetCorrelationId` extension method (or reuse `SetNewCorrelationId`) on `ICorrelationContextAccessor` for Fluxor effects.**
   `CommandEffects<>` / `QueryEffects<>` / `RequestEffects<>` call this extension at the start of their `On*Async` methods (before `BeginCorrelationIdScope`, or folded into it) to explicitly set `accessor.CorrelationContext` from `action.CorrelationId`, so any code executed further down the call stack that reads `ICorrelationContextAccessor` (not just the logging scope) observes the correct correlation ID for the duration of that action's handling.
   *Alternative considered*: Only rely on the logging scope (current behavior) and not explicitly set the accessor. Rejected because the issue explicitly calls for extension methods that let state-management effects set the correlation ID on `ICorrelationContextAccessor`, implying downstream consumers (e.g., HTTP client delegating handlers that read the accessor to set an outgoing correlation header) need the accessor itself populated, not just a logging scope.

5. **Commands/queries in `sample/Pizzeria.Store.Application` drop the `CorrelationId` parameter; endpoints and Fluxor actions are updated to match.**
   Where a domain event still requires an explicit correlation ID (per Non-Goals), the command handler resolves it from `ICorrelationContextAccessor` injected into the handler, rather than from the command.

## Risks / Trade-offs

- [Breaking change for framework consumers] → Documented as **BREAKING** in the proposal; since this is pre-1.0 (`Directory.Build.props` targets .NET 10.0 with no released stable major version per `technology-stack.instructions.md`), the trade-off is acceptable now rather than after wider adoption.
- [`Lewee.Application` gains a direct dependency on `Correlate` abstractions] → Mitigate by depending only on the `ICorrelationContextAccessor` interface (already a minimal, dependency-light contract) rather than pulling in ASP.NET Core-specific `Correlate` middleware types.
- [Fluxor effects setting the accessor could leak correlation IDs across concurrent client-side operations if the accessor is a singleton rather than scoped-per-operation] → Mitigate by verifying `Correlate`'s accessor registration lifetime (it is designed to be scoped/ambient per logical operation) and adding a unit test that asserts the accessor value is set immediately before command/query execution in the effects base classes.
- [Domain events still require an explicit `correlationId` constructor argument] → Acceptable per Non-Goals; this keeps `Lewee.Domain` free of infrastructure dependencies. Application-layer handlers are updated to source the value from the accessor instead of the command.

## Migration Plan

1. Update `Lewee.Application` interfaces (`IApplicationRequest`, `ICommand`, `IQuery<T>`) to drop `CorrelationId`.
2. Update `CorrelationIdLoggingBehavior` to inject and read from `ICorrelationContextAccessor`.
3. Update `Lewee.Infrastructure.FastEndpoints` base classes/extensions.
4. Add/update `Lewee.Infrastructure.Fluxor` extension method(s) and effects base classes to set the accessor explicitly.
5. Update all sample commands/queries, endpoints, and Fluxor actions/effects that reference `CorrelationId` on a command/query.
6. Update existing unit tests (`CorrelationIdLoggingBehaviorTests`, and any sample tests) to construct commands/queries without `CorrelationId` and to supply a fake/mock `ICorrelationContextAccessor` instead.
7. Run `dotnet build --configuration Release --nologo` and `dotnet test --filter "FullyQualifiedName!~Integration"` to confirm no regressions.

No data migration or runtime rollback is required — this is a compile-time API change with no persisted state.

## Open Questions

- Should `IApplicationRequest` be removed entirely (if it has no other members) once `CorrelationId` is removed, or kept as an empty marker interface for future cross-cutting concerns? Decision deferred to implementation; default to removing it if it becomes empty, updating `ICommand`/`IQuery<T>` to no longer reference it.
- Should the `GetCorrelationId()` parsing/fallback logic be extracted into a shared helper referenced by both `Lewee.Infrastructure.FastEndpoints` and `Lewee.Application`, or duplicated in each? Default to extracting a shared extension method to avoid duplicated fallback logic (`Guid.NewGuid()` when unset/unparseable).
