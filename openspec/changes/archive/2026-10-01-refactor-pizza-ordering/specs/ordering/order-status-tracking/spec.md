# Spec Delta

## Purpose

Lets an authenticated customer track a submitted order through its full lifecycle, from the moment it is received to the moment it is completed.

## ADDED Requirements

### Requirement: Customer can view the status of a submitted order

The Ordering Blazor App SHALL display a submitted order's current status as exactly one of: Order received, Making, Ready for pick-up, Ready for delivery, Delivering, or Completed.

#### Scenario: Order has just been submitted

- **WHEN** a customer views an order that has been submitted but not yet marked as prepared
- **THEN** the app SHALL display the status "Order received"

#### Scenario: Order is being prepared

- **WHEN** a customer views an order that has been submitted and the kitchen has begun preparing it but it is not yet marked as prepared
- **THEN** the app SHALL display the status "Making"

#### Scenario: Pickup order is ready

- **WHEN** a customer views a pickup order that has been marked as prepared but not yet completed
- **THEN** the app SHALL display the status "Ready for pick-up"

#### Scenario: Delivery order is ready

- **WHEN** a customer views a delivery order that has been marked as prepared but not yet marked as out for delivery
- **THEN** the app SHALL display the status "Ready for delivery"

#### Scenario: Delivery order is out for delivery

- **WHEN** a customer views a delivery order that has been marked as out for delivery but not yet completed
- **THEN** the app SHALL display the status "Delivering"

#### Scenario: Order is completed

- **WHEN** a customer views an order that has been marked as picked-up or delivered
- **THEN** the app SHALL display the status "Completed"

### Requirement: Customer can only view their own order status

The system SHALL restrict viewing an order's status to the authenticated customer who started that order.

#### Scenario: A different authenticated customer attempts to view another customer's order status

- **WHEN** an authenticated customer requests the status of an order started by a different customer
- **THEN** the system SHALL return a failure result with an unauthorized status and SHALL NOT return the order's details
