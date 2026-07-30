## Purpose

Allows commands and queries to restrict execution to site (super) administrators only, independent of tenant context, so that platform-level operations cannot be invoked by ordinary tenant users.

## ADDED Requirements

### Requirement: Commands and queries can declare administrator-only access
A command or query SHALL be able to declare that it may only be executed by a site administrator. When declared, the framework MUST enforce this restriction before the request reaches its handler.

#### Scenario: Defining an administrator-only command
- **WHEN** a developer defines a command that must only be executable by a site administrator
- **THEN** the command SHALL be able to declare this restriction without any handler-specific authorization code

### Requirement: Non-administrator requests are rejected before handling
When a request declares administrator-only access and the current caller is not a site administrator, the system SHALL reject the request before it reaches the command/query handler and SHALL NOT execute any handler logic or produce side effects.

#### Scenario: Authenticated non-administrator user invokes an administrator-only command
- **WHEN** an authenticated user who is not a site administrator invokes a command that requires administrator access
- **THEN** the system SHALL return a failure result equivalent to an HTTP 403 Forbidden response
- **AND** the command handler SHALL NOT be invoked

#### Scenario: Unauthenticated caller invokes an administrator-only command
- **WHEN** a caller with no authenticated identity invokes a command that requires administrator access
- **THEN** the system SHALL return a failure result equivalent to an HTTP 403 Forbidden response
- **AND** the command handler SHALL NOT be invoked

### Requirement: Administrator requests proceed normally for site administrators
When a request declares administrator-only access and the current caller is a site administrator, the system SHALL allow the request to proceed to its handler unchanged.

#### Scenario: Site administrator invokes an administrator-only command
- **WHEN** an authenticated site administrator invokes a command that requires administrator access
- **THEN** the request SHALL proceed to its handler
- **AND** the resulting response SHALL be unaffected by the administrator-only declaration
