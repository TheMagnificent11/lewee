# Tasks

## 1. Project Renames

- [x] 1.1 Rename directory/project/namespace `sample/Pizzeria.Store.Web` to `sample/Pizzeria.Ordering.Web` (including `.csproj` file name and root namespace) and verify `dotnet build --configuration Release --nologo` succeeds
- [x] 1.2 Rename directory/project/namespace `sample/Pizzeria.Store.StateManagement` to `sample/Pizzeria.Ordering.StateManagement` and verify `dotnet build --configuration Release --nologo` succeeds
- [x] 1.3 Rename directory/project/namespace `sample/Pizzeria.Store.Components` to `sample/Pizzeria.Ordering.Components` and verify `dotnet build --configuration Release --nologo` succeeds
- [x] 1.4 Rename the corresponding test projects `sample-tests/Pizzeria.Store.Web.Tests.Unit`, `sample-tests/Pizzeria.Store.StateManagement.Tests.Unit`, and `sample-tests/Pizzeria.Store.Components.Tests.Unit` to their `Pizzeria.Ordering.*.Tests.Unit` equivalents and verify `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --nologo` passes
- [x] 1.5 Update `lewee.slnx`, `sample/Pizzeria.AppHost/Program.cs`, `sample/Pizzeria.Bff`, and any other `ProjectReference`/solution references to the renamed projects and verify `dotnet build --configuration Release --nologo` succeeds with no dangling references
- [x] 1.6 Run `dotnet format` and verify no outstanding formatting diffs remain after the renames

## 2. Auth Seeding for Pizza Store Roles

- [ ] 2.1 Add a single well-known `Tenant` representing the pizzeria to the existing seeding in `Pizzeria.Configuration` and verify a unit/integration test confirms the tenant exists after seeding
- [ ] 2.2 Add `Store Manager` and `Store Staff` `Role` records (via `Role.Create`) to seeding and verify a test confirms both roles exist with distinct, globally-unique codes
- [ ] 2.3 Extend seeding to create at least one seeded `TenantMembership` for each role for local/dev/test use and verify via a seeding test

## 3. Domain Changes (`Pizzeria.Store.Domain`)

- [ ] 3.1 Add `Order.RemovePizza(Pizza pizza)` (decrementing `OrderPizza.Quantity`, removing the `OrderPizza` at zero) and verify new domain unit tests cover decrement, removal at zero, and removing a pizza not on the order
- [ ] 3.2 Add ownership/state guards to `AddPizza`/`RemovePizza` so they fail once an order `IsSubmitted`, and verify domain unit tests cover both cases
- [ ] 3.3 Add `Menu` add/edit/remove operations for `Pizza` entities (name/price validation) if not already present, and verify domain unit tests cover valid and invalid inputs

## 4. Application Layer - Checkout (`ordering/checkout`)

- [ ] 4.1 Add `RemovePizzaFromOrderCommand` (with validator and handler enforcing the order belongs to the authenticated user) and verify unit tests cover success, not-found pizza, and wrong-owner scenarios from `specs/ordering/checkout/spec.md`
- [ ] 4.2 Add `SubmitPickupOrderCommand` and `SubmitDeliveryOrderCommand` (with validators and handlers enforcing ownership and non-empty order/address) and verify unit tests cover all scenarios in `specs/ordering/checkout/spec.md`
- [ ] 4.3 Add/extend `GetOrderQuery` to return `OrderDto` reflecting current pizzas, quantities, and total cost, and verify unit tests cover populated and empty orders

## 5. Application Layer - Order Status Tracking (`ordering/order-status-tracking`)

- [ ] 5.1 Add a persisted `OrderStatus` enum column to `Order` (Received/Making/ReadyForPickup/ReadyForDelivery/Delivering/Completed), set only via domain methods (e.g. `StartMaking()`, updates to `Prepared()`/`PickedUp()`/`Delivered()`) that validate legal transitions and raise a domain event per transition, exposed on `OrderDto`, and verify unit tests cover every valid transition and rejection of invalid transitions described in `specs/ordering/order-status-tracking/spec.md`
- [ ] 5.2 Add and run the EF Core migration for the new `OrderStatus` column, and verify the migration applies cleanly to a newly provisioned database
- [ ] 5.3 Enforce that `GetOrderQuery`/status retrieval fails with an unauthorized result when the caller does not own the order, and verify a unit test covers this scenario

## 6. Application Layer - Store Order Fulfillment (`pizza-store/order-fulfillment`)

- [ ] 6.1 Add `GetOrdersQuery` supporting a current/past filter (defaulting to current) implementing `ITenantRoleRequest` with Store Staff and Store Manager as satisfying roles, and verify unit tests cover both filter states and unauthorized access per `specs/pizza-store/order-fulfillment/spec.md`
- [ ] 6.2 Add `MarkOrderPizzasPickedUpCommand` and `MarkOrderPizzasDeliveredCommand` (wrapping `Order.PickedUp()`/`Order.Delivered()`) implementing `ITenantRoleRequest`, and verify unit tests cover success, not-yet-prepared failure, and unauthorized-role scenarios

## 7. Application Layer - Menu Management (`pizza-store/menu-management`)

- [ ] 7.1 Add `AddPizzaCommand`, `EditPizzaCommand`, and `RemovePizzaCommand` implementing `ITenantRoleRequest` with only Store Manager as the satisfying role, and verify unit tests cover valid input, not-found, and unauthorized (Store Staff-only) scenarios per `specs/pizza-store/menu-management/spec.md`
- [ ] 7.2 Verify a unit test confirms a caller holding Store Manager can also successfully perform an order-fulfillment operation (roles are additive, not exclusive)

## 8. Pizza Store API (`Pizzeria.Store.Api`)

- [ ] 8.1 Add endpoints for removing a pizza from an order and submitting pickup/delivery checkout, and verify endpoint-level tests/manual `dotnet run` verification against `Pizzeria.Store.Api`
- [ ] 8.2 Add endpoints for listing orders (current/past) and marking pizzas picked-up/delivered, and verify they return `401`/`403`-equivalent failures for callers without the Store Staff/Store Manager role
- [ ] 8.3 Add endpoints for adding, editing, and removing menu pizzas, and verify they return an unauthorized failure for Store Staff-only callers

## 9. Ordering Blazor App (`Pizzeria.Ordering.Web`/`StateManagement`/`Components`)

- [ ] 9.1 Add a pizza-selection/order page showing the menu with "+"/"-" controls that dispatch add/remove actions, and verify a component/unit test in `Pizzeria.Ordering.Components.Tests.Unit` covers add and remove interactions
- [ ] 9.2 Add a checkout page allowing pickup/delivery selection and delivery-address entry, wired to the new submit commands via Fluxor actions/effects/reducers, and verify unit tests cover both pickup and delivery submission flows
- [ ] 9.3 Add an order-status page displaying the persisted `OrderStatus` from `specs/ordering/order-status-tracking/spec.md`, and verify a unit test covers each displayed status value
- [ ] 9.4 Verify `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --nologo` passes for all renamed/updated Ordering projects

## 10. End-to-End Test Coverage (`Pizzeria.Tests.Integration`)

- [ ] 10.1 Following the test pyramid (favour the unit tests added in sections 3-9; add E2E tests only for the primary happy-path and one representative failure path per capability), add a Playwright E2E test covering a customer adding/removing pizzas and completing pickup checkout, and verify it passes against `Pizzeria.Tests.Integration`
- [ ] 10.2 Add a Playwright E2E test covering delivery checkout (including the delivery-address validation failure) and viewing the resulting order status, and verify it passes
- [ ] 10.3 Add a Playwright E2E test covering a Store Staff/Manager user listing current orders and marking an order picked-up/delivered, and verify it passes
- [ ] 10.4 Add a Playwright E2E test covering a Store Manager managing the menu and a Store Staff user being denied that same action, and verify it passes

## 11. Final Verification

- [ ] 11.1 Run `dotnet build --configuration Release --nologo` for the full solution and verify no errors or warnings
- [ ] 11.2 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and verify all unit tests pass
- [ ] 11.3 Run `dotnet test --configuration Release --nologo` (including `Pizzeria.Tests.Integration`) and verify the new E2E tests pass
- [ ] 11.4 Run `dotnet format` and verify there are no outstanding formatting changes
- [ ] 11.5 Manually run `sample/Pizzeria.AppHost` and walk through: start an order, add/remove pizzas, checkout (pickup and delivery), and verify the order status page and store-staff/manager screens reflect the expected behavior
