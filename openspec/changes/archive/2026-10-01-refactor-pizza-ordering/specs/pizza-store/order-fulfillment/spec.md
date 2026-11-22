# Spec Delta

## Purpose

Lets Store Staff and Store Managers view customer orders and progress a submitted order's pizzas through picked-up or out-for-delivery to completion.

## ADDED Requirements

### Requirement: Store staff can view current orders by default

The Pizza Store API SHALL allow a caller holding the Store Staff or Store Manager role to list customer orders, defaulting to orders that have been submitted but not yet completed.

#### Scenario: Store staff requests the order list with no filter

- **WHEN** a caller holding the Store Staff or Store Manager role requests the order list without specifying a status filter
- **THEN** the system SHALL return only orders that are submitted and not yet completed

#### Scenario: Store staff requests past orders

- **WHEN** a caller holding the Store Staff or Store Manager role requests the order list and explicitly asks for past (completed) orders
- **THEN** the system SHALL return only orders that have been completed

### Requirement: Store staff can mark a pickup order's pizzas as picked-up

The system SHALL allow a caller holding the Store Staff or Store Manager role to mark a prepared pickup order's pizzas as picked-up, completing the order.

#### Scenario: Marking a prepared pickup order as picked-up

- **WHEN** a caller holding the Store Staff or Store Manager role marks a pickup order that has been prepared but not yet completed as picked-up
- **THEN** the system SHALL mark the order as completed

#### Scenario: Marking an unprepared order as picked-up

- **WHEN** a caller holding the Store Staff or Store Manager role attempts to mark an order that has not yet been prepared as picked-up
- **THEN** the system SHALL return a failure result and SHALL NOT mark the order as completed

### Requirement: Store staff can mark a delivery order's pizzas as out for delivery

The system SHALL allow a caller holding the Store Staff or Store Manager role to mark a prepared delivery order's pizzas as out for delivery, completing the order once delivered.

#### Scenario: Marking a prepared delivery order as out for delivery

- **WHEN** a caller holding the Store Staff or Store Manager role marks a delivery order that has been prepared but not yet completed as delivered
- **THEN** the system SHALL mark the order as completed

#### Scenario: Marking an unprepared delivery order as delivered

- **WHEN** a caller holding the Store Staff or Store Manager role attempts to mark a delivery order that has not yet been prepared as delivered
- **THEN** the system SHALL return a failure result and SHALL NOT mark the order as completed

### Requirement: Only Store Staff and Store Managers can manage order fulfillment

The Pizza Store API SHALL deny access to the order list and order fulfillment operations to any caller who does not hold the Store Staff or Store Manager role for the pizzeria.

#### Scenario: Caller without a store role attempts to list or fulfill orders

- **WHEN** an authenticated caller who holds neither the Store Staff nor the Store Manager role requests the order list, or attempts to mark an order's pizzas as picked-up or out for delivery
- **THEN** the system SHALL return a failure result with an unauthorized status and SHALL NOT perform the operation
