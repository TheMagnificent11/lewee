## 1. Lewee.Auth.Domain: roles

- [ ] 1.1 Add `Role` as a child entity owned by `Tenant`, with a code unique within that tenant (not globally) and a name; add `Tenant.DefineRole(code, name, correlationId)` (idempotent-by-code within the tenant) and verify with a unit test that duplicate codes within the same tenant are rejected while the same code across different tenants succeeds (`auth/role-management` - "A tenant defines its own roles")
- [ ] 1.2 Add a role-defined domain event (e.g. `RoleDefinedEvent`) raised by `Tenant.DefineRole`, exposing tenant ID, role ID, code, and name, and verify it is raised with a unit test
- [ ] 1.3 Add `TenantMembership.AssignRole(roleId, correlationId)` (idempotent - assigning an already-held role is a no-op) and `RemoveRole(roleId, correlationId)` (idempotent - removing a role not held is a no-op), each validating the role belongs to the membership's own tenant, rejecting cross-tenant role assignment; verify all four behaviors (assign new, assign duplicate, remove held, remove not-held) plus the cross-tenant rejection with unit tests (`auth/role-management`)
- [ ] 1.4 Add `TenantMembershipRoleAssignedEvent`/`TenantMembershipRoleRemovedEvent` domain events raised by `AssignRole`/`RemoveRole` respectively (only when state actually changes), and verify with unit tests that idempotent no-ops do not raise them
- [ ] 1.5 Verify a newly-created `TenantMembership` has zero roles with a unit test (`auth/role-management` - "A newly created tenant membership has no roles")
- [ ] 1.6 Verify that removing a `TenantMembership` (existing `RemoveFromTenant`) also removes its role assignments, with a unit test (`auth/role-management` - "Removing a tenant membership removes its role assignments")
- [ ] 1.7 Promote the reserved administrative tenant code (currently a private constant in `sample/Pizzeria.Configuration/AuthSeeder.cs`) to a shared, public constant on `Lewee.Auth.Domain.Tenant` (or equivalent), and update `AuthSeeder` to reference it, verified by `AuthSeeder`'s existing behavior being unchanged (design.md Decision 1 / Risk 3)

## 2. Lewee.Auth.Infrastructure.Data: persistence

- [ ] 2.1 Add EF Core configuration for `Role` (owned by `Tenant`, unique index on `(TenantId, Code)`) and the `TenantMembership`-to-`Role` assignment (join table under the `auth` schema), and verify with unit tests following the existing patterns in `tests/Lewee.Auth.Infrastructure.Data.Tests.Unit`
- [ ] 2.2 Add the EF Core migration for the new `Role`/membership-role tables and verify it applies cleanly against a fresh `auth` schema (`dotnet ef database update` or equivalent, per `build-and-test.instructions.md`)

## 3. Lewee.Auth.Application: authorization lookup

- [ ] 3.1 Add `TenantMembershipRolesQueryProjection : IQueryProjection` (role codes + `CorrelationId`), keyed by `$"{tenantId}:{externalUserId}"`, per design.md Decision 3
- [ ] 3.2 Add a domain event handler in `Lewee.Auth.Application` that creates/updates/removes the projection (via `IQueryProjectionService` against `AuthDbContext`) in response to `TenantMembershipCreatedEvent`, `TenantMembershipRemovedEvent`, `TenantMembershipRoleAssignedEvent`, and `TenantMembershipRoleRemovedEvent`, and verify each event's effect on the projection with unit tests (`auth/authorization-lookup` scenarios: membership created/removed, role assigned/removed)
- [ ] 3.3 Add `ITenantRoleAuthorizationService` to `Lewee.Application` (contract only - no dependency on `Lewee.Auth.*`), exposing an async "does this external user hold this role for this tenant ID?" check
- [ ] 3.4 Implement `ITenantRoleAuthorizationService` in `Lewee.Auth.Application`, querying the projection from 3.1 via `RetrieveByKeyAsync`, and verify with unit tests covering: existing member with the role, existing member without the role, and non-member (`auth/authorization-lookup` scenarios: looking up an existing member's roles, looking up a non-member)

## 4. Lewee.Application: pipeline behaviors

- [ ] 4.1 Add `IAdministratorRequest` to `Lewee.Application.Mediation.Requests`, exposing the required role, and verify a request implementing it compiles and exposes the role (`application/administrator-authorization` - "Defining an administrator-only command")
- [ ] 4.2 Add `ITenantRoleRequest` (extending `ITenantRequest`) to `Lewee.Application.Mediation.Requests`, exposing the required role in addition to `TenantId`, and verify a request implementing it compiles and exposes both (`application/tenant-role-authorization` - "Defining a tenant-role-restricted command")
- [ ] 4.3 Add `AdministratorAuthorizationBehavior<TRequest, TResponse>` to `Lewee.Application.Mediation.Behaviors`, resolving the caller via `IAuthenticatedUserService`, short-circuiting with `Unauthenticated` when no caller is resolved, checking membership/role in the reserved administrative tenant (task 1.7) via `ITenantRoleAuthorizationService`, and short-circuiting with `Unauthorized` on failure; verify all three outcomes (authorized, unauthorized, unauthenticated) with unit tests (`application/administrator-authorization` - all three pipeline scenarios)
- [ ] 4.4 Add `TenantRoleAuthorizationBehavior<TRequest, TResponse>` to `Lewee.Application.Mediation.Behaviors`, performing the same checks as 4.3 but against `request.TenantId`; verify all four outcomes (authorized, not a member, member without the role, unauthenticated) with unit tests (`application/tenant-role-authorization` - all four pipeline scenarios)
- [ ] 4.5 Verify with unit tests that requests not implementing `IAdministratorRequest`/`ITenantRoleRequest` are unaffected by these behaviors (`application/administrator-authorization` and `application/tenant-role-authorization` - "unaffected" scenarios), and that a request implementing only `ITenantRequest` (not `ITenantRoleRequest`) is not subject to tenant-role authorization
- [ ] 4.6 Register both behaviors as opt-in entries via `ApplicationConfiguration.AddPipelineBehaviors`'s existing `additionalBehaviors` parameter, ordered after `ValidationBehavior` and before `PerformanceBehavior` (design.md Decision 5), and document the registration/opt-in mechanism in `Lewee.Application`'s `README.md`

## 5. Verification

- [ ] 5.1 Search the repo for any existing usage of `ApplicationConfiguration.AddPipelineBehaviors`'s `additionalBehaviors` parameter to confirm the new behaviors' registration does not conflict with host-supplied behaviors
- [ ] 5.2 Run `dotnet build --configuration Release --nologo` and confirm no errors or warnings
- [ ] 5.3 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and confirm all unit tests (including the new ones from sections 1-4) pass
- [ ] 5.4 Run `dotnet format` and confirm code style compliance
- [ ] 5.5 Update `README.md` files for `Lewee.Auth.Domain`, `Lewee.Auth.Application`, and `Lewee.Application` to document `Role`, `ITenantRoleAuthorizationService`, `IAdministratorRequest`, and `ITenantRoleRequest`

## 6. Deferred follow-up (tracked only, not implemented by this plan)

- [ ] 6.1 Design and implement role-management commands/endpoints (e.g. `DefineRoleCommand`, `AssignRoleCommand`, `RemoveRoleCommand`) so tenant administrators can manage roles without direct database access
- [ ] 6.2 Adopt `IAdministratorRequest`/`ITenantRoleRequest` on real Pizzeria sample commands/queries to demonstrate end-to-end authorization behavior
