# Proposal

## Why

The Pizzeria ordering frontend currently uses Blazor Interactive Server, so page interaction and Fluxor state depend on server-side circuits. Moving it to WebAssembly will demonstrate Aspire's supported WASM hosting model and run the customer UI in the browser while retaining the existing API and BFF architecture.

## What Changes

- Replace the ordering app's Interactive Server runtime with a Blazor WebAssembly client hosted and orchestrated by Aspire.
- Move Keycloak sign-in, sign-out, and authenticated API access to the browser using the existing public client and authorization-code flow with PKCE.
- Preserve the ordering experience, BFF-only API access, first-login user provisioning, and live order updates without relying on server circuits or server-only service discovery.
- Update the first-login provisioning flow to run from the WebAssembly client's authenticated state rather than the server's OIDC `OnTokenValidated` event.

## Capabilities

### New Capabilities

- `ordering/browser-client`: The ordering experience runs as an authenticated WebAssembly client and communicates with backend services through browser-accessible BFF endpoints.

### Modified Capabilities

- `auth/user-provisioning`: First-login provisioning is initiated by the WebAssembly client after Keycloak authentication while continuing to use the BFF and the existing idempotent provisioning endpoint.

## Impact

- **Affected projects**: `sample/Pizzeria.Ordering.Web`, `sample/Pizzeria.AppHost`, and `sample-tests/Pizzeria.Tests.Integration`.
- **Affected behavior**: Browser authentication and authorization, first-login provisioning, authenticated Refit requests, SSE order updates, and application hosting.
- **Preserved behavior**: Existing ordering and order-status requirements, backend APIs, BFF routing, database, and Lewee framework packages.
- **Configuration**: Aspire's WebAssembly hosting, Keycloak browser client settings, and browser-reachable BFF URLs must work in development and integration tests without exposing server-only Aspire service names to the browser.
