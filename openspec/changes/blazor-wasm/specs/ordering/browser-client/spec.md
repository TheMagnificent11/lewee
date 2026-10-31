# Spec Delta

## Purpose

Defines the customer ordering experience as an authenticated browser client that does not depend on server-side interactive circuits.

## ADDED Requirements

### Requirement: Ordering interactions run in the browser

The Ordering App SHALL render and process customer interactions in the browser without requiring a server-side interactive circuit.

#### Scenario: Customer loads the ordering app

- **WHEN** a customer opens the Ordering App and its client assets have loaded
- **THEN** the app SHALL render and respond to customer interactions without establishing a server-side interactive circuit

### Requirement: Customers authenticate with Keycloak

The Ordering App SHALL authenticate customers with Keycloak and SHALL require authentication to access customer ordering pages.

#### Scenario: Unauthenticated customer opens an ordering page

- **WHEN** an unauthenticated customer navigates to a protected ordering page
- **THEN** the app SHALL direct the customer to Keycloak sign-in and return them to the requested page after successful authentication

#### Scenario: Authenticated customer signs out

- **WHEN** an authenticated customer signs out of the Ordering App
- **THEN** the app SHALL end the customer's application session and return the browser to the unauthenticated sign-in experience

### Requirement: Browser API calls are restricted to the BFF

The Ordering App SHALL send API requests through the browser-accessible BFF and SHALL NOT expose backend service-discovery addresses to the browser.

#### Scenario: Authenticated customer makes an API request

- **WHEN** the Ordering App sends an API request on behalf of an authenticated customer
- **THEN** the request SHALL be sent to the BFF with the required authorization and SHALL NOT be sent directly to an API service

#### Scenario: API request targets another origin

- **WHEN** an API request is sent from the browser
- **THEN** the customer's access token SHALL NOT be sent to an origin other than the configured BFF origin

### Requirement: Authenticated customers receive live order updates

The Ordering App SHALL receive live order updates through the BFF while an authenticated customer is viewing the ordering experience.

#### Scenario: Authenticated customer receives an order update

- **WHEN** the BFF publishes an order update for an authenticated customer's active session
- **THEN** the Ordering App SHALL process the update and refresh the affected order state

#### Scenario: Customer leaves or signs out

- **WHEN** the customer leaves the Ordering App or signs out
- **THEN** the app SHALL stop its live-update connection and SHALL NOT continue processing updates for that session
