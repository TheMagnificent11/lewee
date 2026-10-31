# Spec Delta

## MODIFIED Requirements

### Requirement: HTTP endpoint provisions a user without requiring authentication

`CreateUserEndpoint` SHALL be hosted by the auth API and accept anonymous requests, allowing a newly authenticated browser client to provision its external identity before application user setup is complete. The Ordering App SHALL invoke the endpoint through the BFF rather than calling the auth API directly.

#### Scenario: First login triggers user provisioning

- **WHEN** a user completes Keycloak sign-in for the first time and the Ordering App submits that user's external ID to the create-user endpoint through the BFF
- **THEN** the endpoint SHALL provision a `User` with no tenant membership and return success without requiring an application session
