## Purpose

Defines how a site administrator defines a global catalog of roles, and how those roles are assigned to and removed from a user's tenant membership, giving pipeline authorization behaviors something concrete to check.

## ADDED Requirements

### Requirement: A site administrator defines a global catalog of roles

`Role`s are defined once, globally - not per `Tenant` - and are available for any `Tenant` to assign to its own members. Each `Role` SHALL be identified by a unique identifier and a code that is unique across the whole system (not merely within a tenant), plus a name that is not required to be unique.

#### Scenario: Defining a role

- **WHEN** a `Role` is defined with a code and a name
- **THEN** the `Role` SHALL be created, assigned a unique identifier, and a role-defined domain event SHALL be raised exposing the role ID, code, and name

#### Scenario: Duplicate role code is rejected

- **WHEN** a `Role` is defined using a code that already exists
- **THEN** persistence SHALL reject the duplicate code, since role codes are unique across the whole system, not per tenant

#### Scenario: Duplicate role name is allowed

- **WHEN** a `Role` is defined using a name that already exists on another `Role`
- **THEN** the `Role` SHALL be created successfully, since only a `Role`'s code is required to be unique, not its name

### Requirement: A newly created tenant membership has no roles

A `TenantMembership` SHALL have zero roles until a `Role` is explicitly assigned to it.

#### Scenario: Assigning a user to a tenant grants no roles

- **WHEN** a `User` is assigned to a `Tenant` (creating a `TenantMembership`)
- **THEN** that `TenantMembership` SHALL have zero roles until a `Role` is explicitly assigned to it

### Requirement: Assigning a role to a tenant membership is idempotent and raises a domain event

Any defined `Role` SHALL be assignable to any `Tenant`'s membership - roles are not scoped to a single tenant. Assigning a `Role` to a `TenantMembership` that already holds it SHALL be a no-op. Assigning a `Role` to a `TenantMembership` that does not already hold it SHALL raise a role-assigned domain event. A `TenantMembership` SHALL be able to hold more than one `Role` at the same time.

#### Scenario: Assigning a new role to an existing tenant membership

- **WHEN** a `Role` is assigned to a `User`'s `TenantMembership` that does not already hold it
- **THEN** the `TenantMembership` SHALL include that `Role` and a role-assigned domain event SHALL be raised

#### Scenario: Assigning the same role twice

- **WHEN** a `User`'s `TenantMembership` is assigned a `Role` it already holds
- **THEN** no duplicate assignment SHALL be created, no additional role-assigned domain event SHALL be raised, and the operation SHALL be treated as an idempotent success

#### Scenario: Assigning multiple roles to a tenant membership

- **WHEN** two different `Role`s are each assigned to the same `User`'s `TenantMembership`
- **THEN** the `TenantMembership` SHALL hold both roles simultaneously

### Requirement: Removing a role from a tenant membership is idempotent and raises a domain event

Removing a `Role` from a `TenantMembership` that holds it SHALL raise a role-removed domain event. Removing a `Role` from a `TenantMembership` that does not hold it SHALL be a no-op.

#### Scenario: Removing an assigned role

- **WHEN** a `Role` held by a `User`'s `TenantMembership` is removed from that membership
- **THEN** the `TenantMembership` SHALL no longer include that `Role` and a role-removed domain event SHALL be raised

#### Scenario: Removing a role that is not held

- **WHEN** a `Role` is removed from a `User`'s `TenantMembership` that does not currently hold it
- **THEN** no role-removed domain event SHALL be raised and the operation SHALL be treated as an idempotent success

#### Scenario: Removing a tenant membership removes its role assignments

- **WHEN** a `User` is removed from a `Tenant` (per `auth/tenant-management`) while their `TenantMembership` holds one or more roles
- **THEN** the role assignments for that membership SHALL be removed along with the membership itself
