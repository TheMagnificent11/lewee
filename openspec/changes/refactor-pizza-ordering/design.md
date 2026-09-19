# Design

## Context

`Pizzeria.Store.Web`, `Pizzeria.Store.StateManagement`, and `Pizzeria.Store.Components` currently implement the customer-facing ordering experience (Blazor Web App + Fluxor state management, per `sample/Pizzeria.Store.Web/AGENTS.md`). `Pizzeria.Store.Domain`/`Application`/`Api`/`Contracts` already contain a minimal `Order`/`Pizza` model (`StartOrder`, `AddPizza`) but no checkout, removal, staff, or menu-management behavior. `Lewee.Auth.*` already provides `Tenant`, `TenantMembership`, `Role`, and the `ITenantRoleRequest`/`TenantRoleAuthorizationBehavior` pipeline (see `authorization-behavior` spec), but the Pizzeria sample app does not yet use any of it - customers are only authenticated via Keycloak with no roles or tenant concept.

## Goals / Non-Goals

**Goals:**

- Rename the three customer-facing projects (and their test projects) from `Pizzeria.Store.*` to `Pizzeria.Ordering.*` without changing their responsibilities.
- Reuse the existing `Order`/`OrderPizza` aggregate and `Lewee` auth/authorization framework rather than inventing new mechanisms.
- Model the pizzeria as a single `Tenant` so `Store Manager`/`Store Staff` roles can be expressed with the existing `ITenantRoleRequest` pipeline instead of a bespoke authorization mechanism.

**Non-Goals:**

- The Pizza Kitchen background service and Pizza Delivery Blazor app described in #572 are out of scope; they are expected to be separate future sub-issues built on top of this change.
- Payment gateway integration is out of scope (per #572, payment is handled out-of-band).
- Multi-store/multi-tenant pizzeria support is out of scope; a single, well-known `Tenant` represents "the pizzeria" for role checks.

## Decisions

### Rename strategy
Rename via `dotnet` tooling (`dotnet sln`/directory + file renames + namespace find/replace) rather than deleting and recreating projects, so git history is preserved. Update `lewee.slnx`, `sample/Pizzeria.AppHost/Program.cs`, `sample/Pizzeria.Bff` references, and the three `sample-tests/Pizzeria.Store.*.Tests.Unit` projects (renamed to `Pizzeria.Ordering.*.Tests.Unit`) in the same change to keep the solution buildable at every step.

### Single pizzeria tenant for role checks
**Decision**: Seed one well-known `Tenant` (e.g. via `Pizzeria.Configuration` seeding, alongside existing auth seeding) representing the pizzeria, and define `Store Manager` and `Store Staff` as `Role`s assignable via `TenantMembership` to that tenant. Pizza Store API commands/queries implement `ITenantRoleRequest` using that tenant's ID and the roles that satisfy the request.
**Alternative considered**: Add a bespoke `IsStoreManager`/`IsStoreStaff` flag directly on `User`, mirroring `IsSiteAdministrator`. Rejected because the repo already has a generic, tested tenant-role authorization pipeline (`authorization-behavior`); duplicating it with ad hoc flags would fragment authorization logic and contradict the `Role`/`TenantMembership` model already adopted for auth.

### Order removal and checkout on the existing aggregate
**Decision**: Extend `Order`/`OrderPizza` with `RemovePizza` (decrementing/removing `OrderPizza`) and keep using the existing `SubmitPickupOrder`/`SubmitDeliveryOrder` methods for checkout, adding ownership checks (`Order.UserId` must match the caller) at the command-handler level via `IAuthenticatedUserService`, consistent with `StartOrderCommand`.
**Alternative considered**: Introduce a separate `Cart` aggregate distinct from `Order`, converted to an `Order` at checkout. Rejected as unnecessary complexity - the existing `Order` aggregate already models an in-progress, mutable order prior to submission.

### Order status derivation
**Decision**: Derive the customer-facing status (`Order received`, `Making`, `Ready for pick-up`/`Ready for delivery`, `Delivering`, `Completed`) from the existing `Order` timestamp fields (`SubmittedDateTime`, `PreparedDateTime`, `CompletedDateTime`) and `IsDeliveryOrder`, computed in the query/DTO layer (`Pizzeria.Store.Contracts`/`Pizzeria.Store.Application`) rather than persisting a separate status enum. `Making` vs `Order received` distinguishes on whether kitchen processing has started; since no kitchen service exists yet in this change, `Making` is derived from the same `SubmittedDateTime`/`PreparedDateTime` gap and will be refined once the Pizza Kitchen background service (future work) introduces an explicit "in progress" signal.
**Alternative considered**: Add an explicit `OrderStatus` enum column now. Deferred because the kitchen service's data needs are not yet defined; adding a persisted enum before that design exists risks having to migrate again.

### Store-staff order fulfillment endpoints
**Decision**: Add `PizzasPickedUp`/`PizzasDelivered` command handlers (wrapping the existing `Order.PickedUp()`/`Order.PizzasDelivered()` domain methods) and a `GetOrdersQuery` supporting a status filter (current vs past), all implementing `ITenantRoleRequest` with `Store Staff` and `Store Manager` as satisfying roles. Menu management commands (`AddPizza`, `EditPizza`, `RemovePizza` on `Menu`/`Pizza`) implement `ITenantRoleRequest` with only `Store Manager` as the satisfying role.

## Risks / Trade-offs

- **[Risk]** Introducing tenant/role concepts into the sample app for the first time increases the seeding and setup surface (a `Tenant`, two `Role`s, and `TenantMembership` records must exist before store staff can authenticate). **Mitigation**: Extend the existing `Pizzeria.Configuration` seeder (which already seeds Keycloak/auth data) rather than introducing a new seeding mechanism.
- **[Risk]** Deriving `Making` status without kitchen-service input is a placeholder that may need revisiting once the Pizza Kitchen background service exists. **Mitigation**: Documented as an explicit design decision above and scoped as a likely follow-up rather than left implicit.
- **[Trade-off]** Renaming three projects touches many files (namespaces, `csproj` names, directories, solution file, `AppHost` references) purely for clarity, with no functional change; this is accepted as a one-time cost to keep naming consistent as the Pizza Store API and future Kitchen/Delivery apps are built out.

## Migration Plan

1. Rename projects/directories/namespaces first, in a self-contained step that builds and passes existing tests before any new functionality is added.
2. Add the single pizzeria `Tenant` and `Store Manager`/`Store Staff` roles to seeding.
3. Add domain/application changes (`RemovePizza`, order-fulfillment commands, menu-management commands) incrementally, each with tests, followed by the corresponding API endpoints.
4. Add the Blazor UI (checkout, order status, store staff/manager screens) last, once the underlying API surface is in place.

There is no data to migrate for existing environments since the sample app has no production deployment; local/dev databases are recreated via EF Core migrations as part of normal sample app setup.
