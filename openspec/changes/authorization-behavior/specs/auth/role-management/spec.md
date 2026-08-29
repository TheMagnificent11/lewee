## Purpose

Defines how roles - a fixed, application-defined set of string codes - are assigned to and removed from a user's tenant membership, giving pipeline authorization behaviors something concrete to check.

## ADDED Requirements

### Requirement: A newly created tenant membership has no roles
A `TenantMembership` SHALL have zero roles until a role is explicitly assigned to it. Roles are simple string codes; `Lewee.Auth.Domain` does not define, own, or validate a catalog of roles - each hosting application chooses which role codes are meaningful to it.

#### Scenario: Assigning a user to a tenant grants no roles
- **WHEN** a `User` is assigned to a `Tenant` (creating a `TenantMembership`)
- **THEN** that `TenantMembership` SHALL have zero roles until a role is explicitly assigned to it

### Requirement: Assigning a role to a tenant membership is idempotent and raises a domain event
Assigning a role code to a `TenantMembership` that already holds it SHALL be a no-op. Assigning a role code to a `TenantMembership` that does not already hold it SHALL raise a role-assigned domain event. A `TenantMembership` SHALL be able to hold more than one role code at the same time.

#### Scenario: Assigning a new role to an existing tenant membership
- **WHEN** a role code is assigned to a `User`'s `TenantMembership` that does not already hold it
- **THEN** the `TenantMembership` SHALL include that role code and a role-assigned domain event SHALL be raised

#### Scenario: Assigning the same role twice
- **WHEN** a `User`'s `TenantMembership` is assigned a role code it already holds
- **THEN** no duplicate assignment SHALL be created, no additional role-assigned domain event SHALL be raised, and the operation SHALL be treated as an idempotent success

#### Scenario: Assigning multiple roles to a tenant membership
- **WHEN** two different role codes are each assigned to the same `User`'s `TenantMembership`
- **THEN** the `TenantMembership` SHALL hold both role codes simultaneously

### Requirement: Removing a role from a tenant membership is idempotent and raises a domain event
Removing a role code from a `TenantMembership` that holds it SHALL raise a role-removed domain event. Removing a role code from a `TenantMembership` that does not hold it SHALL be a no-op.

#### Scenario: Removing an assigned role
- **WHEN** a role code held by a `User`'s `TenantMembership` is removed from that membership
- **THEN** the `TenantMembership` SHALL no longer include that role code and a role-removed domain event SHALL be raised

#### Scenario: Removing a role that is not held
- **WHEN** a role code is removed from a `User`'s `TenantMembership` that does not currently hold it
- **THEN** no role-removed domain event SHALL be raised and the operation SHALL be treated as an idempotent success

#### Scenario: Removing a tenant membership removes its role assignments
- **WHEN** a `User` is removed from a `Tenant` (per `auth/tenant-management`) while their `TenantMembership` holds one or more role codes
- **THEN** the role assignments for that membership SHALL be removed along with the membership itself
