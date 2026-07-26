## ADDED Requirements

### Requirement: Commands and queries do not declare an explicit correlation ID
Commands (`ICommand`) and queries (`IQuery<T>`) SHALL NOT require a `CorrelationId` constructor parameter or property. The MediatR pipeline and any handler that needs the current correlation ID MUST resolve it from `ICorrelationContextAccessor` rather than from the request object.

#### Scenario: Defining a new command without a correlation ID parameter
- **WHEN** a developer defines a new command record implementing `ICommand`
- **THEN** the command SHALL compile without declaring a `CorrelationId` parameter or property

#### Scenario: Defining a new query without a correlation ID parameter
- **WHEN** a developer defines a new query record implementing `IQuery<T>`
- **THEN** the query SHALL compile without declaring a `CorrelationId` parameter or property

### Requirement: MediatR pipeline resolves correlation ID from the ambient accessor
`CorrelationIdLoggingBehavior` SHALL resolve the correlation ID for its logging scope from `ICorrelationContextAccessor` instead of from the request. If the accessor has no correlation ID set, the behavior SHALL generate a new `Guid` for the logging scope rather than failing.

#### Scenario: Correlation ID present on the accessor
- **WHEN** a command or query is dispatched through the MediatR pipeline and `ICorrelationContextAccessor.CorrelationContext.CorrelationId` contains a valid GUID string
- **THEN** `CorrelationIdLoggingBehavior` SHALL use that GUID as the `CorrelationId` value in its logging scope

#### Scenario: Correlation ID absent from the accessor
- **WHEN** a command or query is dispatched through the MediatR pipeline and `ICorrelationContextAccessor.CorrelationContext` is null or does not contain a parseable GUID
- **THEN** `CorrelationIdLoggingBehavior` SHALL generate a new `Guid` and use it as the `CorrelationId` value in its logging scope, and SHALL still invoke the next pipeline delegate

### Requirement: FastEndpoints command/query endpoints construct requests without a correlation ID argument
`CommandEndpoint<TRequest>` and `QueryEndpoint<T>` base classes, and any derived endpoint, SHALL construct command/query instances without passing a `CorrelationId` argument, relying on the `Correlate` middleware to populate `ICorrelationContextAccessor` from the incoming HTTP request before the endpoint executes.

#### Scenario: Handling an incoming HTTP request with a correlation ID header
- **WHEN** an HTTP request carrying a correlation ID header reaches a FastEndpoints command or query endpoint
- **THEN** the endpoint SHALL construct its command or query without a `CorrelationId` constructor argument, and the correlation ID SHALL remain available via `ICorrelationContextAccessor` for downstream pipeline behaviors

### Requirement: Fluxor effects explicitly propagate the action's correlation ID onto the ambient accessor
`CommandEffects<>`, `QueryEffects<>`, and `RequestEffects<>` base classes SHALL set `ICorrelationContextAccessor.CorrelationContext` from the current `IRequestAction.CorrelationId` before executing the command/query, so that any code invoked during effect execution which reads `ICorrelationContextAccessor` observes the action's correlation ID, not only the logging scope.

#### Scenario: Executing a command effect
- **WHEN** a Fluxor `CommandEffects<>` subclass handles an `IRequestAction` via `OnCommandAsync`
- **THEN** `ICorrelationContextAccessor.CorrelationContext.CorrelationId` SHALL equal the action's `CorrelationId` for the duration of the command execution

#### Scenario: Executing a query effect
- **WHEN** a Fluxor `QueryEffects<>` subclass handles an `IRequestAction` via `OnQueryAsync`
- **THEN** `ICorrelationContextAccessor.CorrelationContext.CorrelationId` SHALL equal the action's `CorrelationId` for the duration of the query execution
