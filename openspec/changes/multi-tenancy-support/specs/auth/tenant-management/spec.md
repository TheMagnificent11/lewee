## Purpose

Defines the `Tenant` aggregate root and the invariants governing how `User` entities are owned by, and scoped to, a single `Tenant`, so that authorization data can be modeled per-organisation in the application database instead of in Keycloak.

## ADDED Requirements

### Requirement: Tenant is the aggregate root for authorization data
`Tenant` SHALL be an aggregate root (deriving from `Lewee.Domain.AggregateRoot`) that owns a collection of `User` entities. `User` SHALL NOT be an aggregate root; it SHALL be a child entity that can only be created, retrieved, and persisted in the context of its owning `Tenant`.

#### Scenario: Creating a tenant
- **WHEN** a new `Tenant` is created with a name
- **THEN** the `Tenant` SHALL be assigned a unique identifier and SHALL raise a tenant-created domain event

#### Scenario: User cannot exist without a tenant
- **WHEN** a `User` is constructed
- **THEN** the `User` SHALL require a valid, non-empty `TenantId` referencing an existing `Tenant`, and construction SHALL fail if no tenant context is supplied

### Requirement: Users are unique per tenant by external identity
Within a single `Tenant`, each `User`'s external identity (the Keycloak subject/user identifier) SHALL be unique. The same external identity MAY belong to `User` records in different tenants without conflict.

#### Scenario: Duplicate external ID within the same tenant
- **WHEN** a second `User` is created for the same `Tenant` with an external ID that already exists for that `Tenant`
- **THEN** the operation SHALL NOT create a duplicate `User` record and SHALL be treated as an idempotent success (matching the existing `CreateCustomerCommand` "already exists" behavior)

#### Scenario: Same external ID across different tenants
- **WHEN** a `User` is created for `Tenant` A with external ID `X`, and a separate `User` is created for `Tenant` B with the same external ID `X`
- **THEN** both `User` records SHALL be created successfully as distinct users scoped to their respective tenants

### Requirement: Tenant-scoped queries exclude other tenants' data
Any query or specification that resolves `User` entities SHALL be scoped to a specific `Tenant`, so that data belonging to one tenant is never returned when resolving another tenant's users.

#### Scenario: Querying a user by external ID within a tenant
- **WHEN** a caller queries for a `User` by external ID and `TenantId`
- **THEN** only the `User` belonging to that `TenantId` (if any) SHALL be returned, even if a `User` with the same external ID exists under a different `Tenant`
