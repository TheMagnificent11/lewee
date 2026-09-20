# Proposal

## Why

The Pizzeria sample app currently only demonstrates a bare-bones "start order / add pizza" flow with no checkout, no order status visibility, and no way for store staff to manage orders or the menu. `Pizzeria.Store.Web`, `Pizzeria.Store.StateManagement`, and `Pizzeria.Store.Components` are also named as if the store staff worked in them, when they actually implement the **customer-facing ordering experience**, which is confusing now that a separate Pizza Store API/back-office capability is being built out (#572). Renaming these projects to `Pizzeria.Ordering.*` and completing the ordering/store-management functionality is required before the Pizza Kitchen and Delivery apps (future sub-issues of #572) can be built on top of a stable, well-named foundation.

## What Changes

- **BREAKING**: Rename `Pizzeria.Store.Web` project (and namespace) to `Pizzeria.Ordering.Web`.
- **BREAKING**: Rename `Pizzeria.Store.StateManagement` project (and namespace) to `Pizzeria.Ordering.StateManagement`.
- **BREAKING**: Rename `Pizzeria.Store.Components` project (and namespace) to `Pizzeria.Ordering.Components`.
- Update `lewee.slnx`, `Pizzeria.AppHost`, `Pizzeria.Bff`, and any other project references/`ProjectReference`s, `csproj` names, and associated test projects to match the renamed projects.
- Add customer checkout functionality to the Ordering Blazor App: reviewing the current order, choosing pickup or delivery, supplying a delivery address when required, removing/decreasing pizzas, and submitting the order.
- Add an order status screen to the Ordering Blazor App showing the order lifecycle (Order received, Making, Ready for pick-up/Ready for delivery, Delivering, Completed).
- Add Pizza Store API functionality: store staff can view current and past customer orders (defaulting to current), mark an order's pizzas as picked-up, and mark an order's pizzas as out for delivery.
- Add Pizza Store API menu management functionality restricted to the Store Manager role: add, edit, and remove pizzas from the menu. Store Managers retain all Store Staff permissions.
- Introduce `Store Manager` and `Store Staff` roles (using the existing role-based authorization behavior from `authorization-behavior`) so Pizza Store API endpoints can be locked down appropriately, while the Ordering Blazor App remains available to any authenticated `Customer`.

## Capabilities

### New Capabilities

- `ordering/checkout`: Customers reviewing, adjusting (including removing pizzas), and submitting an order for pickup or delivery.
- `ordering/order-status-tracking`: Customers viewing the current status of their order through its full lifecycle.
- `pizza-store/order-fulfillment`: Store staff viewing current/past orders and progressing an order's pizzas through picked-up/out-for-delivery states.
- `pizza-store/menu-management`: Store managers adding, editing, and removing pizzas from the menu, with store staff denied access.

### Modified Capabilities

- None. `authorization-behavior` already defines role-based authorization; this change only assigns the `Store Manager`/`Store Staff` roles to the new endpoints and does not alter its requirements.

## Impact

- **Renamed projects**: `sample/Pizzeria.Store.Web` → `sample/Pizzeria.Ordering.Web`, `sample/Pizzeria.Store.StateManagement` → `sample/Pizzeria.Ordering.StateManagement`, `sample/Pizzeria.Store.Components` → `sample/Pizzeria.Ordering.Components`, plus their associated test projects.
- **Affected configuration**: `lewee.slnx`, `sample/Pizzeria.AppHost/Program.cs`, `sample/Pizzeria.Bff`, `Directory.Build.props`/`.editorconfig` references that key off the old project names.
- **Affected code**: `Pizzeria.Store.Domain` (`Order`, `OrderPizza`), `Pizzeria.Store.Application` (new/updated commands and queries for checkout, removal, staff order management, menu CRUD), `Pizzeria.Store.Api` (new endpoints), `Pizzeria.Store.Contracts` (DTOs/routes), and the renamed Blazor app/state-management/components projects (new pages, actions, reducers).
- **Auth**: New `Store Manager` and `Store Staff` roles need to be seeded/creatable via the existing `Role` entity so Pizza Store API endpoints can require them.
