# Spec Delta

## Purpose

Lets an authenticated customer review the pizzas in their in-progress order, adjust quantities, remove pizzas, and submit the order for pickup or delivery.

## ADDED Requirements

### Requirement: Customer can view their in-progress order

The Ordering Blazor App SHALL display the authenticated customer's current in-progress order, including each pizza, its quantity, and the order's running total cost.

#### Scenario: Customer views an in-progress order with pizzas

- **WHEN** an authenticated customer navigates to their in-progress order and it contains one or more pizzas
- **THEN** the app SHALL display each pizza with its quantity and the order's total cost

#### Scenario: Customer views an in-progress order with no pizzas

- **WHEN** an authenticated customer navigates to their in-progress order and it contains no pizzas
- **THEN** the app SHALL indicate the order is empty and SHALL prevent checkout until at least one pizza is added

### Requirement: Customer can remove a pizza from an in-progress order

The system SHALL allow a customer to decrease the quantity of a pizza already in their in-progress order, removing it entirely once its quantity reaches zero.

#### Scenario: Decreasing quantity of a pizza with more than one remaining

- **WHEN** a customer decreases the quantity of a pizza that currently has a quantity greater than one in their in-progress order
- **THEN** the system SHALL decrement that pizza's quantity by one and leave the pizza in the order

#### Scenario: Removing the last unit of a pizza

- **WHEN** a customer decreases the quantity of a pizza that currently has a quantity of one in their in-progress order
- **THEN** the system SHALL remove that pizza from the order entirely

#### Scenario: Removing a pizza not in the order

- **WHEN** a customer attempts to remove a pizza that is not part of their in-progress order
- **THEN** the system SHALL return a failure result and SHALL NOT modify the order

### Requirement: Customer can choose pickup or delivery at checkout

The system SHALL allow a customer to submit their in-progress order as either a pickup order or a delivery order requiring a non-empty delivery address.

#### Scenario: Submitting a pickup order

- **WHEN** a customer submits an in-progress order containing at least one pizza and chooses pickup
- **THEN** the system SHALL mark the order as submitted with no delivery address and SHALL prevent further additions or removals of pizzas

#### Scenario: Submitting a delivery order with a valid address

- **WHEN** a customer submits an in-progress order containing at least one pizza, chooses delivery, and supplies a non-empty delivery address
- **THEN** the system SHALL mark the order as submitted with the supplied delivery address and SHALL prevent further additions or removals of pizzas

#### Scenario: Submitting a delivery order without an address

- **WHEN** a customer chooses delivery but supplies an empty or whitespace-only delivery address
- **THEN** the system SHALL return a failure result indicating the delivery address is required and SHALL NOT submit the order

#### Scenario: Submitting an order with no pizzas

- **WHEN** a customer attempts to submit an order that contains no pizzas
- **THEN** the system SHALL return a failure result and SHALL NOT submit the order

#### Scenario: Submitting an already-submitted order

- **WHEN** a customer attempts to submit an order that has already been submitted
- **THEN** the system SHALL return a failure result and SHALL NOT change the order's existing submission details

### Requirement: Only the owning customer can modify their order

The system SHALL restrict adding pizzas to, removing pizzas from, and submitting an order to the authenticated customer who started that order.

#### Scenario: A different authenticated customer attempts to modify another customer's order

- **WHEN** an authenticated customer attempts to add, remove, or submit pizzas on an order started by a different customer
- **THEN** the system SHALL return a failure result with an unauthorized status and SHALL NOT modify the order
