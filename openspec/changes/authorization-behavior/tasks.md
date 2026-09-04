## 1. Lewee.Auth.Domain: site administrator and roles

- [x] 1.1 Add `IsSiteAdministrator` (boolean, defaulting to `false`) to `User`, with an `internal` setter (exposed to trusted callers via `InternalsVisibleTo` - see design.md Decision 1); verify the default with a unit test (`application/tenant-role-authorization` reads it for the site-administrator bypass, `auth/role-management` is unaffected by it)
- [x] 1.2 Add `Role` as a root entity (not owned by `Tenant`), with a globally-unique code and a name; add `Role.Create(code, name, correlationId)` (idempotent-by-code) and verify with a unit test that a duplicate code is rejected (`auth/role-management` - "A site administrator defines a global catalog of roles")
- [x] 1.3 Add a role-defined domain event (e.g. `RoleDefinedEvent`) raised by `Role.Create`, exposing role ID, code, and name, and verify it is raised with a unit test
- [x] 1.4 Add `TenantMembership.AssignRole(roleId, correlationId)` (idempotent - assigning an already-held role is a no-op) and `RemoveRole(roleId, correlationId)` (idempotent - removing a role not held is a no-op), with no per-tenant ownership check (any defined `Role` may be assigned to any tenant's membership); verify all four behaviors (assign new, assign duplicate, remove held, remove not-held) with unit tests (`auth/role-management`)
- [x] 1.5 Add `TenantRoleAssignedEvent`/`TenantRoleRemovedEvent` domain events raised by `AssignRole`/`RemoveRole` respectively (only when state actually changes), and verify with unit tests that idempotent no-ops do not raise them
- [x] 1.6 Verify a newly-created `TenantMembership` has zero roles with a unit test (`auth/role-management` - "A newly created tenant membership has no roles")
- [x] 1.7 Verify a `TenantMembership` can hold more than one role at once, with a unit test (`auth/role-management` - "Assigning multiple roles to a tenant membership")
- [x] 1.8 Verify that removing a `TenantMembership` (existing `RemoveFromTenant`) also removes its role assignments, with a unit test (`auth/role-management` - "Removing a tenant membership removes its role assignments")

## 2. Lewee.Auth.Infrastructure.Data: persistence

- [x] 2.1 Add EF Core configuration for `User.IsSiteAdministrator`, the `Role` entity (unique index on `Code`), and the `TenantMembership`-to-`Role` assignment (join table under the `auth` schema), and verify with unit tests following the existing patterns in `tests/Lewee.Auth.Infrastructure.Data.Tests.Unit`
- [x] 2.2 Add the EF Core migration for the new `Role`/membership-role tables and `IsSiteAdministrator` column, and verify it applies cleanly against a fresh `auth` schema (`dotnet ef database update` or equivalent, per `build-and-test.instructions.md`)

## 3. Lewee.Auth.Application: authorization lookup

- [x] 3.1 Add `TenantMembershipRolesQueryProjection : IQueryProjection` (role codes + `CorrelationId`), keyed by `$"{tenantId}:{externalUserId}"`, per design.md Decision 3
- [x] 3.2 Add a domain event handler in `Lewee.Auth.Application` that creates/updates/removes the projection (via `IQueryProjectionService` against `AuthDbContext`) in response to `TenantMembershipCreatedEvent`, `TenantMembershipRemovedEvent`, `TenantRoleAssignedEvent`, and `TenantRoleRemovedEvent`, and verify each event's effect on the projection with unit tests (`auth/authorization-lookup` scenarios: membership created/removed, role assigned/removed)

## 4. Lewee.Auth.Application: pipeline behaviors

- [x] 4.1 Add `ITenantRoleRequest` (extending `Lewee.Application.Mediation.Requests.ITenantRequest`) to `Lewee.Auth.Application`, exposing the set of roles that satisfy the request (`Roles`) in addition to `TenantId`, and verify a request implementing it compiles and exposes both (`application/tenant-role-authorization` - "Defining a tenant-role-restricted command")
- [x] 4.2 Move `TenantLoggingBehavior` from `Lewee.Application.Mediation.Behaviors` to `Lewee.Auth.Application`, updating its namespace and any references, and verify existing unit tests for it still pass after the move
- [x] 4.3 Add `TenantRoleAuthorizationBehavior<TRequest, TResponse>` to `Lewee.Auth.Application`, resolving the caller via `IAuthenticatedUserService`, short-circuiting with `Unauthenticated` when no caller is resolved, then looking up the caller's `User` by external ID and bypassing straight to the handler when `IsSiteAdministrator` is `true` (design.md Decision 1 - a site administrator has super-user access to every tenant-scoped request); otherwise checking (via the authorization lookup from section 3) whether the caller is a member of `request.TenantId` and holds at least one of the request's required roles, and short-circuiting with `Unauthorized` on failure; verify all outcomes (site-administrator bypass, authorized, not a member, member without any required role, unauthenticated) with unit tests (`application/tenant-role-authorization` - all pipeline scenarios)
- [x] 4.4 Verify with unit tests that requests not implementing `ITenantRoleRequest` are unaffected by this behavior (`application/tenant-role-authorization` - "unaffected" scenario), and that a request implementing only `ITenantRequest` (not `ITenantRoleRequest`) is not subject to tenant-role authorization
- [x] 4.5 Add `ApplicationAuthConfiguration` to `Lewee.Auth.Application` (distinct from the existing `AuthApplicationConfiguration`), exposing a method that registers `TenantLoggingBehavior` and `TenantRoleAuthorizationBehavior` as opt-in entries via `ApplicationConfiguration.AddPipelineBehaviors`'s existing `additionalBehaviors` parameter, ordered after `ValidationBehavior` and before `PerformanceBehavior` (design.md Decision 5); verify with a unit test that calling it registers both behaviors
- [x] 4.6 Add `CachedUserRepository` decorating `IRepository<User>` with an `IMemoryCache`-backed cache of `RetrieveByIdAsync`/`QueryOneAsync(UserByExternalIdSpecification)` lookups (design.md Decision 6), and have `ApplicationAuthConfiguration` register `IMemoryCache` and replace the host's `IRepository<User>` registration with a factory wrapping it in `CachedUserRepository`; verify cache-hit and pass-through-for-other-specifications behavior with unit tests

## 5. sample/Pizzeria.Configuration: remove the reserved ADMIN tenant

- [x] 5.1 Remove `AuthSeeder`'s reserved `"ADMIN"` tenant creation and the seeded admin user's assignment to it; set the seeded admin user's `IsSiteAdministrator` to `true` directly instead, and verify `AuthSeeder`'s updated behavior with a unit test
- [x] 5.2 Confirm no other sample code depends on the removed `"ADMIN"` tenant (search the repo); no data migration is required since the sample's database can simply be recreated (design.md Migration Plan step 7)

## 6. Verification

- [x] 6.1 Search the repo for any existing usage of `ApplicationConfiguration.AddPipelineBehaviors`'s `additionalBehaviors` parameter, and any direct references to `Lewee.Application`'s `TenantLoggingBehavior`, to confirm the move/new registration does not conflict with host-supplied behaviors
- [x] 6.2 Run `dotnet build --configuration Release --nologo` and confirm no errors or warnings
- [x] 6.3 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and confirm all unit tests (including the new ones from sections 1-5) pass
- [x] 6.4 Run `dotnet format` and confirm code style compliance
- [x] 6.5 Update `README.md` files for `Lewee.Auth.Domain` and `Lewee.Auth.Application` to document `IsSiteAdministrator`, role assignment, `ITenantRoleRequest`, `ApplicationAuthConfiguration`, and `CachedUserRepository`

## 7. Deferred follow-up (tracked only, not implemented by this plan)

- [ ] 7.1 Design and implement role-management commands/endpoints (e.g. `DefineRoleCommand` restricted to a site administrator, `AssignRoleCommand`/`RemoveRoleCommand` restricted to a site administrator and/or a tenant's own manager-equivalent role) so roles can be defined and assigned without direct database access
- [ ] 7.2 Adopt `ITenantRoleRequest` on real Pizzeria sample commands/queries to demonstrate end-to-end authorization behavior
