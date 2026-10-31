# Design

## Context

See `proposal.md` for motivation and the spec deltas for the required browser behavior.

`Pizzeria.Ordering.Web` is currently an ASP.NET Core Blazor Web App using Interactive Server rendering. Its Refit and SSE clients use `AuthTokenDelegatingHandler`, which obtains tokens from `IHttpContextAccessor`; its API base addresses and SSE endpoint use Aspire service names that are not browser-resolvable. Order reconciliation cancellation is tied to a Blazor Server circuit. The AppHost already runs Aspire 13.6.4, and the Keycloak realm already defines `pizzeria-store-web` as a public authorization-code client with PKCE.

## Goals / Non-Goals

**Goals:**

- Run the existing customer-facing UI and Fluxor state in a standalone WebAssembly client.
- Keep Keycloak as the identity provider and the BFF as the only browser-facing backend boundary.
- Preserve first-login user provisioning and authenticated live order updates.
- Continue to run the client and backend services from the Aspire AppHost and exercise them through the existing Playwright integration tests.

**Non-Goals:**

- Change ordering, authorization, API, database, or BFF business behavior.
- Add a server-side interactive render mode or retain server circuits as a fallback.
- Change Lewee framework behavior beyond the dependency separation required to make the state-management and component graph browser-compatible.

## Decisions

### Host a standalone WebAssembly client through Aspire's Blazor Gateway

Convert `Pizzeria.Ordering.Web` to a standalone Blazor WebAssembly project and register it with Aspire's `AddBlazorWasmProject` resource. Attach the resource to an Aspire Blazor Gateway using `WithBlazorClientApp`; reference the existing BFF as the client API service so the gateway supplies a browser-reachable, same-origin proxy route. Use the existing AppHost Aspire version and centrally manage the Blazor hosting integration package alongside the other Aspire packages. The gateway becomes the public URL used by the app and browser tests; the BFF remains responsible for routing to the Auth and Store APIs.

This uses Aspire's dedicated WASM hosting rather than keeping the existing ASP.NET Core Web App as a server-rendering host. The Aspire Blazor hosting API is experimental, so keep its use localized to the AppHost and handle its diagnostic using the repository's analyzer-suppression convention rather than adding a `#pragma` suppression.

**Alternative considered:** Keep the existing web host and enable Interactive WebAssembly rendering. Rejected because it retains a server host and mixed render-mode concerns when the issue requires the application to be fully WASM.

### Separate browser and server dependencies

The retained components and state-management graph currently reaches `Lewee.Infrastructure.Auth` through `Lewee.Infrastructure.Fluxor`, which brings in `Microsoft.AspNetCore.App`; that framework has no `browser-wasm` runtime pack, so the standalone client cannot build with it. State management also references the server `Lewee.Auth.Api` assembly for `CreateUserRequest`. Split the browser-safe pieces (Fluxor/SSE client abstractions) from the server-only authentication infrastructure so the client graph has no `Microsoft.AspNetCore.App` reference, and move the provisioning request contract into a browser-safe contracts assembly shared by the client and `Lewee.Auth.Api`. This is a required framework and sample change, not a conditional one, and must be done before conversion and build verification.

### Authenticate in the browser and restrict tokens to the BFF

Configure the WebAssembly client for Keycloak authorization-code authentication with PKCE, reusing the `pizzeria-store-web` public client and its existing API audience mappings. Do not add a client secret. Register browser-side authentication and authorization services, and configure Refit `HttpClient` instances with the gateway's same-origin BFF route and an authorization handler limited to that BFF origin. Do not reuse `AddWebApiHttpClient` or `AuthTokenDelegatingHandler` in the browser: they depend on server service discovery and `IHttpContextAccessor`.

Browser Refit clients must also preserve correlation-header propagation (the role of `CorrelatingHttpMessageHandler` in `AddWebApiHttpClient`), because the Fluxor flow matches SSE events to pending commands by the outgoing correlation ID.

After Keycloak authentication supplies the external user ID, initiate the existing idempotent create-user request from the authenticated client through the BFF. Keep the endpoint's anonymous access and its existing no-tenant behavior; do not call the Auth API directly from the browser.

**Alternative considered:** Put an OIDC client secret in the WebAssembly app or call the backend APIs directly. Rejected because browser assets and configuration are public, and the existing BFF is the established backend boundary.

### Adapt the live-update transport and lifecycle for WebAssembly

Keep the existing Fluxor actions, reducers, message mapping, and order-reconciliation behavior. Register browser-compatible HTTP transports for the Refit API client and `SseClientMessageReceiver`, using the gateway's BFF route and the same origin-restricted bearer-token handler. Replace the server `OrderReconciliationCircuitHandler` registration with a client-lifetime cancellation provider so pending work and the SSE stream stop when the authenticated client session ends.

Validate that the WebAssembly HTTP transport can stream SSE responses through the gateway. If its `HttpClient` transport cannot stream in the target browser, use browser fetch streaming with the authorization header; do not place access tokens in query strings or use an unauthenticated event URL. Keep the existing `ClientEventReceiver` message mapping and stop/dispose behavior. `AddStoreState` calls `AddLeweeFluxor`, which registers an `IAuthenticatedUserService` backed by `IHttpContextAccessor`; in WebAssembly its `UserId` would be empty and `ClientEventReceiver` would never start SSE after sign-in. Provide a browser-backed `IAuthenticatedUserService` populated from the browser authentication state, and ensure state registration does not overwrite it.

### Keep browser verification on the public gateway

Update the integration-test factory to discover and use the gateway URL rather than the WASM resource's internal build/runtime endpoint. Continue using Playwright to cover sign-in, sign-out, first-login provisioning, order creation and checkout, and live order updates. Add checks that the UI works without a server-side circuit and that API traffic is routed through the gateway to the BFF. Keep the existing domain and state-management unit tests as regression coverage.

## Risks / Trade-offs

- **[Risk]** Aspire's Blazor hosting API is experimental and may change independently of the current AppHost SDK. **Mitigation:** Keep the integration localized to AppHost, use the repository-managed Aspire version, and validate it with a focused AppHost build and integration startup.
- **[Risk]** Browser OIDC redirects and the Keycloak public client's allowed origins may differ between development and integration-test gateway URLs. **Mitigation:** Verify the actual gateway callback and sign-out URLs in Playwright and configure the realm for the required browser URLs without adding a client secret.
- **[Risk]** SSE streaming or token attachment may behave differently in WebAssembly browsers than with server `HttpClient`. **Mitigation:** Prove authenticated SSE through the gateway early; use fetch streaming with an authorization header if required, and test reconnect and cancellation behavior.
- **[Risk]** The existing `auth/user-provisioning` main spec uses a delta-only `## ADDED Requirements` heading, so OpenSpec cannot archive this modified-capability delta until the baseline heading is normalized. **Mitigation:** Change only that heading to `## Requirements` when applying the delta and validate the spec before archive.
- **[Trade-off]** The standalone client moves UI execution and authentication state to the browser. The BFF and APIs remain server-side, but browser networking and identity configuration become explicit.

## Migration Plan

1. Add and configure Aspire's Blazor hosting integration, register the standalone WASM client and gateway, and confirm the gateway serves a minimal app in the existing Aspire topology.
2. Move the ordering UI and shared components/state references into the WebAssembly client; remove Interactive Server services, render modes, circuit handler, and server-only registrations.
3. Configure browser Keycloak authentication, BFF-scoped API clients, and first-login user provisioning.
4. Adapt and verify authenticated SSE transport and client-lifetime cancellation.
5. Update the integration-test factory and Playwright flows to target the gateway; run sample build, unit tests, and integration tests.
6. Normalize the existing user-provisioning main-spec heading while applying the delta, then validate the OpenSpec change before archive.

Rollback is a source-control revert of the sample client and AppHost migration; no database or backend API migration is required.
