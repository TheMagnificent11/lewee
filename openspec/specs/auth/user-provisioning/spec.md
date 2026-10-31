## Purpose

Defines how a `User` is provisioned from an external (Keycloak) identity, replacing the sample application's `CreateCustomerCommand`/`CreateCustomerEndpoint` flow with a framework-level `CreateUserCommand`/`CreateUserEndpoint`. Provisioning a `User` is independent of tenant assignment: a `User` is created with no tenant membership, and is assigned to one or more tenants by separate, later functionality.

## Requirements

### Requirement: Creating a user requires only an external identity

A `CreateUserCommand` SHALL accept the external user identifier (from the authenticated Keycloak principal). The command SHALL fail validation if the external user identifier is missing, empty, or exceeds its maximum length. The command SHALL NOT require or accept a `TenantId`.

#### Scenario: Valid create-user request

- **WHEN** `CreateUserCommand` is dispatched with a non-empty external user ID
- **THEN** the command SHALL succeed and a `User` SHALL exist with the supplied external ID and zero tenant memberships

#### Scenario: Missing external user ID

- **WHEN** `CreateUserCommand` is dispatched with an empty or missing external user ID
- **THEN** validation SHALL fail and no `User` SHALL be created

### Requirement: Creating a user is idempotent

If a `User` with the given external ID already exists, `CreateUserCommand` SHALL return a successful result without creating a duplicate record.

#### Scenario: Repeated first-login

- **WHEN** `CreateUserCommand` is dispatched twice with the same external user ID
- **THEN** only one `User` record SHALL exist for that external ID, and both dispatches SHALL report success

### Requirement: HTTP endpoint provisions a user without requiring authentication

`CreateUserEndpoint` SHALL be hosted by the auth API and accept anonymous requests, allowing a newly authenticated browser client to provision its external identity before application user setup is complete. The Ordering App SHALL invoke the endpoint through the BFF rather than calling the auth API directly.

#### Scenario: First login triggers user provisioning

- **WHEN** a user completes Keycloak sign-in for the first time and the Ordering App submits that user's external ID to the create-user endpoint through the BFF
- **THEN** the endpoint SHALL provision a `User` with no tenant membership and return a success response, without requiring the caller to already hold an application session
