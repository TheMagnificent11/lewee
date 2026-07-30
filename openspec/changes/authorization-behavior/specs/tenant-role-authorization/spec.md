## Purpose

Allows commands and queries to restrict execution to callers holding one or more specific roles within the current tenant, resolved from application-owned authorization data rather than identity-provider claims.

## ADDED Requirements

### Requirement: Commands and queries can declare required tenant role(s)
A command or query that also identifies a tenant SHALL be able to declare one or more tenant roles required to execute it. When declared, the framework MUST enforce that the current caller holds at least one of the required roles for that tenant before the request reaches its handler.

#### Scenario: Defining a tenant-role-restricted command
- **WHEN** a developer defines a command that must only be executable by callers holding a specific role within the command's tenant
- **THEN** the command SHALL be able to declare the required role(s) without any handler-specific authorization code

### Requirement: Caller role membership is resolved from application-owned data
The system SHALL determine whether the current caller holds a required tenant role by looking up role assignments stored in the application's own database, keyed by tenant ID and user ID, rather than by reading role/claim information from the identity provider's authentication token.

#### Scenario: Role lookup for an authenticated caller
- **WHEN** a request declaring required tenant role(s) is dispatched for an authenticated caller
- **THEN** the system SHALL resolve the caller's roles for the request's tenant from the application's authorization data store before allowing or denying the request

### Requirement: Requests are rejected when the caller lacks a required role
When a request declares required tenant role(s) and the current caller does not hold any of those roles for the request's tenant, the system SHALL reject the request before it reaches the command/query handler and SHALL NOT execute any handler logic or produce side effects.

#### Scenario: Caller lacks the required tenant role
- **WHEN** an authenticated caller without any of the required roles for the tenant invokes a role-restricted command
- **THEN** the system SHALL return a failure result equivalent to an HTTP 403 Forbidden response
- **AND** the command handler SHALL NOT be invoked

#### Scenario: Caller holds a role for a different tenant
- **WHEN** an authenticated caller holds a required role for a tenant other than the tenant identified on the request
- **THEN** the system SHALL return a failure result equivalent to an HTTP 403 Forbidden response
- **AND** the command handler SHALL NOT be invoked

### Requirement: Requests proceed when the caller holds a required role
When a request declares required tenant role(s) and the current caller holds at least one of those roles for the request's tenant, the system SHALL allow the request to proceed to its handler unchanged.

#### Scenario: Caller holds a required tenant role
- **WHEN** an authenticated caller holding one of the required roles for the request's tenant invokes a role-restricted command
- **THEN** the request SHALL proceed to its handler
- **AND** the resulting response SHALL be unaffected by the role-restriction declaration

### Requirement: Role assignment changes are reflected in subsequent authorization checks
When a user's tenant role assignments are created, changed, or removed, the system SHALL update the authorization data used for role lookups so that subsequent requests reflect the caller's current roles without requiring a service restart.

#### Scenario: Role granted to a user
- **WHEN** a tenant administrator grants a user a new role for a tenant
- **THEN** a subsequent role-restricted request from that user for that tenant SHALL be authorized according to the newly granted role

#### Scenario: Role revoked from a user
- **WHEN** a tenant administrator revokes a user's role for a tenant
- **THEN** a subsequent role-restricted request from that user for that tenant that required the revoked role SHALL be rejected
