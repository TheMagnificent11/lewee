## Purpose

Defines how a `User` is provisioned for a `Tenant` from an external (Keycloak) identity, replacing the sample application's `CreateCustomerCommand`/`CreateCustomerEndpoint` flow with a framework-level `CreateUserCommand`/`CreateUserEndpoint`.

## ADDED Requirements

### Requirement: Creating a user requires an external identity and a tenant
A `CreateUserCommand` SHALL accept the external user identifier (from the authenticated Keycloak principal) and the target `TenantId`. The command SHALL fail validation if either value is missing, empty, or exceeds the external identifier's maximum length.

#### Scenario: Valid create-user request
- **WHEN** `CreateUserCommand` is dispatched with a non-empty external user ID and a valid `TenantId`
- **THEN** the command SHALL succeed and a `User` SHALL exist for that `Tenant` with the supplied external ID

#### Scenario: Missing external user ID
- **WHEN** `CreateUserCommand` is dispatched with an empty or missing external user ID
- **THEN** validation SHALL fail and no `User` SHALL be created

### Requirement: Creating a user is idempotent per tenant
If a `User` with the given external ID already exists for the given `Tenant`, `CreateUserCommand` SHALL return a successful result without creating a duplicate record.

#### Scenario: Repeated first-login for the same tenant
- **WHEN** `CreateUserCommand` is dispatched twice with the same external user ID and the same `TenantId`
- **THEN** only one `User` record SHALL exist for that `Tenant`, and both dispatches SHALL report success

### Requirement: HTTP endpoint provisions a user without requiring authentication
`CreateUserEndpoint` SHALL accept anonymous requests (mirroring the existing `CreateCustomerEndpoint` behavior), since it is invoked during the authentication handshake (`OnTokenValidated`) before the caller has an established application session.

#### Scenario: First login triggers user provisioning
- **WHEN** a user completes Keycloak authentication for the first time and the web application's `OnTokenValidated` handler calls the create-user endpoint with that user's external ID
- **THEN** the endpoint SHALL provision a `User` for the configured tenant and return a success response, without requiring the caller to already hold an application session
