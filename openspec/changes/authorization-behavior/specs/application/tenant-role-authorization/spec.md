## Purpose

Defines how commands and queries restricted to specific tenant roles are authorized in the `MediatR` pipeline for the tenant the request pertains to.

## ADDED Requirements

### Requirement: Commands and queries opt into tenant-role authorization via a marker interface

A command (`ICommand`) or query (`IQuery<T>`) that must only be executed by a caller holding at least one of a set of roles within the tenant it pertains to SHALL implement `ITenantRoleRequest`, which extends the existing `ITenantRequest` and additionally exposes the set of roles that satisfy the request. Requests that do not implement `ITenantRoleRequest` SHALL NOT be subject to this authorization check, even if they implement `ITenantRequest`.

#### Scenario: Defining a tenant-role-restricted command

- **WHEN** a developer defines a command or query implementing `ITenantRoleRequest`
- **THEN** the request SHALL compile by exposing the tenant ID (via `ITenantRequest`) and the set of roles that satisfy the request, and the `MediatR` pipeline SHALL authorize it before it reaches its handler

#### Scenario: A request that implements only ITenantRequest is unaffected

- **WHEN** a command or query implements `ITenantRequest` but not `ITenantRoleRequest`
- **THEN** tenant-role authorization SHALL NOT be evaluated for that request, and it SHALL proceed to its handler unaffected by this capability

### Requirement: The pipeline authorizes tenant-role requests against the request's own tenant

For a request implementing `ITenantRoleRequest`, the pipeline SHALL resolve the current caller's external identity, determine (via `auth/authorization-lookup`) whether that caller is a member of `request.TenantId` and holds at least one of the request's satisfying roles for that tenant, and SHALL only invoke the handler when that check succeeds. A caller holding any one of the request's satisfying roles is sufficient - the caller need not hold all of them.

#### Scenario: Authorized tenant member

- **WHEN** a caller who is a member of `request.TenantId` and holds at least one of the request's satisfying roles for that tenant dispatches an `ITenantRoleRequest` command or query
- **THEN** the pipeline SHALL invoke the handler and return its result

#### Scenario: Caller is not a member of the tenant

- **WHEN** a caller who is not a member of `request.TenantId` dispatches an `ITenantRoleRequest` command or query
- **THEN** the pipeline SHALL short-circuit before the handler and return a failure result with an unauthorized status, and SHALL NOT invoke the handler

#### Scenario: Caller is a member but holds none of the required roles

- **WHEN** a caller who is a member of `request.TenantId` but holds none of the request's satisfying roles dispatches an `ITenantRoleRequest` command or query
- **THEN** the pipeline SHALL short-circuit before the handler and return a failure result with an unauthorized status, and SHALL NOT invoke the handler

#### Scenario: No authenticated caller

- **WHEN** an `ITenantRoleRequest` command or query is dispatched with no resolvable authenticated caller
- **THEN** the pipeline SHALL short-circuit before the handler and return a failure result with an unauthenticated status, and SHALL NOT invoke the handler
