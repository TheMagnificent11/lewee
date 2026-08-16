## Purpose

Defines the `Tenant` aggregate root and the invariants governing how a `User` (itself an aggregate root) may be a member of zero, one, or more `Tenant`s, so that authorization data can be modeled per-organisation in the application database instead of in Keycloak.

## ADDED Requirements

### Requirement: Tenant is its own aggregate root
`Tenant` SHALL be an aggregate root (deriving from `Lewee.Domain.AggregateRoot`), independent of `User`. `Tenant` SHALL NOT own or require the existence of any `User` at creation time.

#### Scenario: Creating a tenant
- **WHEN** a new `Tenant` is created with a name
- **THEN** the `Tenant` SHALL be assigned a unique identifier and SHALL raise a tenant-created domain event

### Requirement: User remains an aggregate root and may belong to zero or more tenants
`User` SHALL remain an aggregate root (deriving from `Lewee.Domain.AggregateRoot`), constructed independently of any `Tenant`. A `User` SHALL be able to exist as a member of zero tenants (e.g. immediately after creation), and SHALL be able to be a member of one or more tenants via an explicit membership relationship.

#### Scenario: A newly-created user has no tenant membership
- **WHEN** a `User` is created
- **THEN** the `User` SHALL have zero tenant memberships until explicitly assigned to a `Tenant`

#### Scenario: A user can be assigned to more than one tenant
- **WHEN** a `User` is assigned to `Tenant` A and subsequently assigned to `Tenant` B
- **THEN** the `User` SHALL have memberships for both `Tenant` A and `Tenant` B, and neither membership SHALL affect the other

### Requirement: Assigning a user to a tenant is idempotent and raises a domain event
Assigning a `User` to a `Tenant` it already belongs to SHALL NOT create a duplicate membership record. Assigning a `User` to a `Tenant` it is not already a member of SHALL raise a `TenantMembershipCreatedEvent`.

#### Scenario: Assigning the same user to the same tenant twice
- **WHEN** a `User` is assigned to a `Tenant` it is already a member of
- **THEN** the operation SHALL NOT create a duplicate membership, SHALL NOT raise another `TenantMembershipCreatedEvent`, and SHALL be treated as an idempotent success

#### Scenario: Assigning a user to a new tenant raises an event
- **WHEN** a `User` is assigned to a `Tenant` it is not already a member of
- **THEN** a `TenantMembership` SHALL be created and a `TenantMembershipCreatedEvent` SHALL be raised

### Requirement: Removing a user from a tenant is idempotent and raises a domain event
Removing a `User` from a `Tenant` it belongs to SHALL delete the corresponding membership record and raise a `TenantMembershipRemovedEvent`. Removing a `User` from a `Tenant` it is not a member of SHALL NOT raise an event and SHALL be treated as an idempotent success.

#### Scenario: Removing an existing membership raises an event
- **WHEN** a `User` who is a member of a `Tenant` is removed from that `Tenant`
- **THEN** the corresponding `TenantMembership` SHALL be deleted and a `TenantMembershipRemovedEvent` SHALL be raised

#### Scenario: Removing a membership that does not exist is a no-op
- **WHEN** a `User` who is not a member of a `Tenant` is removed from that `Tenant`
- **THEN** no `TenantMembership` SHALL be deleted, no `TenantMembershipRemovedEvent` SHALL be raised, and the operation SHALL be treated as an idempotent success

### Requirement: Users are unique by external identity
Each `User`'s external identity (the Keycloak subject/user identifier) SHALL be unique across the whole system, independent of tenant membership.

#### Scenario: Duplicate external ID
- **WHEN** a second `User` is created with an external ID that already exists
- **THEN** the operation SHALL NOT create a duplicate `User` record and SHALL be treated as an idempotent success (matching the existing `CreateCustomerCommand` "already exists" behavior)
