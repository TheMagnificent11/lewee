## Purpose

Defines how commands and queries restricted to a site administrator are authorized in the `MediatR` pipeline, irrespective of the tenant the request pertains to.

## ADDED Requirements

### Requirement: Commands and queries opt into administrator-only authorization via a marker interface
A command (`ICommand`) or query (`IQuery<T>`) that must only be executed by a site administrator SHALL implement `IAdministratorRequest`, exposing the role required within the reserved administrative tenant. Requests that do not implement `IAdministratorRequest` SHALL NOT be subject to this authorization check.

#### Scenario: Defining an administrator-only command
- **WHEN** a developer defines a command or query implementing `IAdministratorRequest`
- **THEN** the request SHALL compile by exposing the required administrator role, and the `MediatR` pipeline SHALL authorize it before it reaches its handler

#### Scenario: A request that does not implement IAdministratorRequest is unaffected
- **WHEN** a command or query does not implement `IAdministratorRequest`
- **THEN** administrator authorization SHALL NOT be evaluated for that request, and it SHALL proceed to its handler unaffected by this capability

### Requirement: The pipeline authorizes administrator requests against the reserved administrative tenant
For a request implementing `IAdministratorRequest`, the pipeline SHALL resolve the current caller's external identity, determine (via `auth/authorization-lookup`) whether that caller holds the request's required role within the reserved administrative tenant, and SHALL only invoke the handler when that check succeeds.

#### Scenario: Authorized administrator
- **WHEN** a caller who is a member of the reserved administrative tenant and holds the request's required role dispatches an `IAdministratorRequest` command or query
- **THEN** the pipeline SHALL invoke the handler and return its result

#### Scenario: Caller lacks the required administrator role
- **WHEN** a caller who is not a member of the reserved administrative tenant, or is a member but does not hold the request's required role, dispatches an `IAdministratorRequest` command or query
- **THEN** the pipeline SHALL short-circuit before the handler and return a failure result with an unauthorized status, and SHALL NOT invoke the handler

#### Scenario: No authenticated caller
- **WHEN** an `IAdministratorRequest` command or query is dispatched with no resolvable authenticated caller
- **THEN** the pipeline SHALL short-circuit before the handler and return a failure result with an unauthenticated status, and SHALL NOT invoke the handler
