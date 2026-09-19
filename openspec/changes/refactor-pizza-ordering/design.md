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

### Order status as a persisted column set by domain methods

**Decision**: Add a persisted `OrderStatus` enum column (`Received`, `Making`, `ReadyForPickup`, `ReadyForDelivery`, `Delivering`, `Completed`) to `Order`, set only via domain methods (e.g. `StartMaking()`, `PizzasPrepared()`, `PickedUp()`, `PizzasDelivered()`) that validate the current/next-status transition and raise a domain event for each transition (e.g. `OrderMakingStartedEvent`, extending the existing `OrderPreparedEvent`/`PickupOrderSubmittedEvent`/`DeliveryOrderSubmittedEvent` pattern) so side effects (kitchen processing, notifications, read-model projections) can react to status changes instead of polling timestamps. `Pizzeria.Store.Contracts`/`Pizzeria.Store.Application` expose `OrderStatus` on `OrderDto` directly from this column.
**Alternative considered**: Derive status from existing timestamp fields (`SubmittedDateTime`, `PreparedDateTime`, `CompletedDateTime`) and `IsDeliveryOrder` in the query/DTO layer. Rejected per reviewer feedback: deriving status prevents domain events from being raised on each transition (needed for future side effects like the Pizza Kitchen background service) and conflates "no explicit status recorded yet" with genuine status values.

### Store-staff order fulfillment endpoints

**Decision**: Add `PizzasPickedUp`/`PizzasDelivered` command handlers (wrapping the existing `Order.PickedUp()`/`Order.PizzasDelivered()` domain methods) and a `GetOrdersQuery` supporting a status filter (current vs past), all implementing `ITenantRoleRequest` with `Store Staff` and `Store Manager` as satisfying roles. Menu management commands (`AddPizza`, `EditPizza`, `RemovePizza` on `Menu`/`Pizza`) implement `ITenantRoleRequest` with only `Store Manager` as the satisfying role.

## Risks / Trade-offs

- **[Risk]** Introducing tenant/role concepts into the sample app for the first time increases the seeding and setup surface (a `Tenant`, two `Role`s, and `TenantMembership` records must exist before store staff can authenticate). **Mitigation**: Extend the existing `Pizzeria.Configuration` seeder (which already seeds Keycloak/auth data) rather than introducing a new seeding mechanism.
- **[Risk]** Who/what triggers the `Making` transition is not yet defined, since the Pizza Kitchen background service is out of scope for this change. **Mitigation**: Expose a domain method (e.g. `StartMaking()`) now so the future kitchen service has a clear, event-raising entry point to call; until that service exists, the Pizza Store API can expose the same method to store staff/manager as a manual trigger.
- **[Trade-off]** Renaming three projects touches many files (namespaces, `csproj` names, directories, solution file, `AppHost` references) purely for clarity, with no functional change; this is accepted as a one-time cost to keep naming consistent as the Pizza Store API and future Kitchen/Delivery apps are built out.

## Migration Plan

1. Rename projects/directories/namespaces first, in a self-contained step that builds and passes existing tests before any new functionality is added.
2. Add the single pizzeria `Tenant` and `Store Manager`/`Store Staff` roles to seeding.
3. Add domain/application changes (`OrderStatus` column and domain methods/events, `RemovePizza`, order-fulfillment commands, menu-management commands) incrementally, each with tests, followed by the corresponding API endpoints.
4. Add the Blazor UI (checkout, order status, store staff/manager screens) last, once the underlying API surface is in place.

A new database will be provisioned for this change, so no existing data needs to be migrated; the `OrderStatus` column and its EF Core migration only need to account for new rows going forward.
