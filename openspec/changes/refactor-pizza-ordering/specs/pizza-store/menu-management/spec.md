# Spec Delta

## Purpose

Lets Store Managers maintain the pizza menu that customers order from, while keeping menu changes off-limits to Store Staff.

## ADDED Requirements

### Requirement: Store manager can add a pizza to the menu
The Pizza Store API SHALL allow a caller holding the Store Manager role to add a new pizza to the menu by specifying its name and price.

#### Scenario: Adding a pizza with valid details
- **WHEN** a caller holding the Store Manager role adds a pizza with a non-empty name and a price greater than zero
- **THEN** the system SHALL add the pizza to the menu so it becomes available for customers to order

#### Scenario: Adding a pizza with invalid details
- **WHEN** a caller holding the Store Manager role attempts to add a pizza with an empty name or a price that is zero or negative
- **THEN** the system SHALL return a failure result and SHALL NOT add the pizza to the menu

### Requirement: Store manager can edit an existing pizza on the menu
The system SHALL allow a caller holding the Store Manager role to update an existing pizza's name and/or price.

#### Scenario: Editing a pizza with valid details
- **WHEN** a caller holding the Store Manager role updates an existing pizza with a non-empty name and a price greater than zero
- **THEN** the system SHALL update that pizza's details

#### Scenario: Editing a pizza that does not exist
- **WHEN** a caller holding the Store Manager role attempts to update a pizza that does not exist on the menu
- **THEN** the system SHALL return a failure result with a not-found status

### Requirement: Store manager can remove a pizza from the menu
The system SHALL allow a caller holding the Store Manager role to remove an existing pizza from the menu so it is no longer available for new orders.

#### Scenario: Removing a pizza that exists on the menu
- **WHEN** a caller holding the Store Manager role removes a pizza that currently exists on the menu
- **THEN** the system SHALL remove the pizza so it is no longer returned to customers browsing the menu

#### Scenario: Removing a pizza that does not exist
- **WHEN** a caller holding the Store Manager role attempts to remove a pizza that does not exist on the menu
- **THEN** the system SHALL return a failure result with a not-found status

### Requirement: Only Store Managers can change the menu
The Pizza Store API SHALL deny menu add, edit, and remove operations to any caller who does not hold the Store Manager role for the pizzeria, including callers who hold only the Store Staff role.

#### Scenario: Store staff attempts to change the menu
- **WHEN** a caller holding only the Store Staff role attempts to add, edit, or remove a pizza on the menu
- **THEN** the system SHALL return a failure result with an unauthorized status and SHALL NOT change the menu

#### Scenario: Store manager performs a store-staff operation
- **WHEN** a caller holding the Store Manager role performs an order-fulfillment operation that Store Staff are also permitted to perform
- **THEN** the system SHALL allow the operation, since Store Managers retain all Store Staff permissions
