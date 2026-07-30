## Purpose

Establishes the application-owned representation of a user and their identity-provider linkage, independent of any specific sample application, so that authorization data (site-administrator status and tenant roles) can be attributed to a durable, framework-level user record.

## ADDED Requirements

### Requirement: Users are represented independently of the identity provider
The system SHALL maintain its own record of a user, keyed by an internal identifier, that is linked to (but distinct from) the identity provider's representation of that user via an external identifier. Authorization data (site-administrator status, tenant role assignments) SHALL be stored against the system's own user record and SHALL NOT depend on claims issued by the identity provider.

#### Scenario: A user record exists independently of identity provider claims
- **WHEN** a user authenticates via the identity provider
- **THEN** the system SHALL resolve the corresponding application user record via the external identifier
- **AND** any authorization decision for that user SHALL be based on data held in the application's own user/role records, not on claims present in the identity provider's token

### Requirement: A new identity-provider user results in a corresponding application user
When a person authenticates through the identity provider for the first time and no corresponding application user record exists for their external identifier, the system SHALL create an application user record linked to that external identifier.

#### Scenario: First-time authentication creates a user record
- **WHEN** a person authenticates successfully via the identity provider and no application user record exists for their external identifier
- **THEN** the system SHALL create a new application user record associated with that external identifier
- **AND** subsequent authentications by the same external identifier SHALL resolve to the same application user record without creating a duplicate

### Requirement: An initial site administrator user is provisioned on application startup
The system SHALL ensure that at least one site administrator user exists, provisioning one automatically if none exists, so that tenant and role management can be bootstrapped without manual database intervention.

#### Scenario: Application starts with no existing administrator user
- **WHEN** the application starts and no site administrator user exists in the application's data store
- **THEN** the system SHALL provision an administrator identity in the identity provider (if one does not already exist) and create a corresponding application user record marked as a site administrator
