## Purpose

Defines tenant-scoped roles that a tenant can define and assign to a user's tenant membership, giving pipeline authorization behaviors something concrete to check.

## ADDED Requirements

### Requirement: A tenant defines its own roles
`Tenant` SHALL be able to define zero or more `Role`s, each identified by a code that is unique within that tenant (but not necessarily globally unique) and a name. A `Role` SHALL only apply within the `Tenant` that defined it.

#### Scenario: Defining a role
- **WHEN** a `Tenant` defines a new `Role` with a code and name
- **THEN** the `Role` SHALL be created scoped to that tenant, assigned a unique identifier, and a role-defined domain event SHALL be raised exposing the tenant ID, role ID, code, and name

#### Scenario: Duplicate role code within the same tenant
- **WHEN** a `Tenant` defines a second `Role` using a code that already exists for that tenant
- **THEN** persistence SHALL reject the duplicate code

#### Scenario: Same role code reused across different tenants
- **WHEN** two different `Tenant`s each define a `Role` with the same code
- **THEN** both roles SHALL be created successfully, since role codes are unique per tenant, not globally

### Requirement: A newly created tenant membership has no roles
A `TenantMembership` SHALL have zero roles until a `Role` is explicitly assigned to it.

#### Scenario: Assigning a user to a tenant grants no roles
- **WHEN** a `User` is assigned to a `Tenant` (creating a `TenantMembership`)
- **THEN** that `TenantMembership` SHALL have zero roles until a `Role` is explicitly assigned to it

### Requirement: Assigning a role to a tenant membership is idempotent and raises a domain event
A `Role` SHALL only be assignable to a `TenantMembership` for the same `Tenant` that defined the `Role`. Assigning a `Role` to a `TenantMembership` that already holds it SHALL be a no-op. Assigning a `Role` to a `TenantMembership` that does not already hold it SHALL raise a role-assigned domain event.

#### Scenario: Assigning a new role to an existing tenant membership
- **WHEN** a `User` who is already a member of a `Tenant` is assigned a `Role` defined by that same `Tenant`
- **THEN** the `TenantMembership` SHALL include that `Role` and a role-assigned domain event SHALL be raised

#### Scenario: Assigning the same role twice
- **WHEN** a `User`'s `TenantMembership` is assigned a `Role` it already holds
- **THEN** no duplicate assignment SHALL be created, no additional role-assigned domain event SHALL be raised, and the operation SHALL be treated as an idempotent success

#### Scenario: Assigning a role from a different tenant is rejected
- **WHEN** an attempt is made to assign a `Role` defined by `Tenant` A to a `User`'s `TenantMembership` for `Tenant` B
- **THEN** the operation SHALL be rejected, since a `Role` only applies within the `Tenant` that defined it

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
