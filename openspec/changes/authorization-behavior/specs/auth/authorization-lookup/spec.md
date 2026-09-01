## Purpose

Defines a fast, tenant-and-user-keyed lookup of a caller's roles that pipeline authorization behaviors can query with a single read, kept in sync with tenant membership and role changes.

## ADDED Requirements

### Requirement: Authorization lookup resolves a caller's roles for a tenant without a per-request join
The system SHALL provide a lookup, keyed by a user's external identity and a tenant ID, that resolves to the set of role codes the user holds for that tenant (per `auth/role-management`). Consumers (such as pipeline authorization behaviors) SHALL be able to query this lookup with a single key-based read rather than joining membership and role-assignment tables at request time.

#### Scenario: Looking up an existing member's roles
- **WHEN** the authorization lookup is queried with the external identity of a `User` who is a member of a `Tenant`, for that tenant's ID
- **THEN** it SHALL return the set of role codes currently held by that `TenantMembership`, or an empty set if none are assigned

#### Scenario: Looking up a non-member
- **WHEN** the authorization lookup is queried with the external identity of a `User` who is not a member of the specified `Tenant`
- **THEN** it SHALL indicate no membership (an empty or absent result) rather than an error

### Requirement: The lookup stays in sync with tenant membership and role changes
Whenever a `TenantMembership` is created or removed (per `auth/tenant-management`), or a role assignment on a `TenantMembership` changes (per `auth/role-management`), the authorization lookup SHALL be updated to reflect the new state before that change is considered complete, so a subsequent authorization check observes the change.

#### Scenario: Membership created
- **WHEN** a `User` is assigned to a `Tenant`
- **THEN** the authorization lookup SHALL reflect that the user is a member of that tenant, with zero roles, before any role is separately assigned

#### Scenario: Membership removed
- **WHEN** a `User` is removed from a `Tenant`
- **THEN** the authorization lookup SHALL no longer report that user as a member of that tenant, and SHALL no longer return any of that membership's previously assigned role codes

#### Scenario: Role assigned
- **WHEN** a role code is assigned to a `User`'s `TenantMembership`
- **THEN** the authorization lookup for that user and tenant SHALL include the newly assigned role code

#### Scenario: Role removed
- **WHEN** a role code is removed from a `User`'s `TenantMembership`
- **THEN** the authorization lookup for that user and tenant SHALL no longer include the removed role code
