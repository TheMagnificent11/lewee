# Lewee.Auth.Contracts

Browser-safe HTTP contracts shared by Lewee authentication APIs and clients. The package has no
server-specific dependencies and can be referenced by Blazor WebAssembly applications.

## Features

- Defines the request contract for idempotent user provisioning
- Has no package or framework dependencies

## Main Types

| Type | Purpose |
|------|---------|
| `CreateUserRequest` | Contains the external user identifier to provision |

## Usage

The auth API and browser client share this request type:

```csharp
var request = new CreateUserRequest
{
    ExternalUserId = externalUserId,
};
```

`Lewee.Auth.Api` hosts the endpoint. Browser clients should submit this request through the BFF.
