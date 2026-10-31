# Tasks

## 1. Aspire Hosting and WebAssembly Client

- [ ] 1.1 Add Aspire's Blazor hosting integration and register the standalone WASM client, existing BFF reference, and public gateway in the AppHost; verify the AppHost builds and serves the client through the gateway.
- [ ] 1.2 Convert `Pizzeria.Ordering.Web` to a standalone WebAssembly project and remove Interactive Server render modes and server-only startup services; verify the sample solution builds and the browser UI works without a server-side circuit.
- [ ] 1.3 Update `sample/Pizzeria.Ordering.Web/AGENTS.md`, `.github/instructions/sample-application.instructions.md`, and `.github/instructions/repository-structure.instructions.md` to describe the WASM client and gateway; verify the edited Markdown passes `markdownlint`.

## 2. Browser Authentication and API Access

- [ ] 2.1 Configure browser-side Keycloak authorization-code authentication with PKCE, protected routes, and sign-in/sign-out navigation using the public `pizzeria-store-web` client; verify Playwright covers unauthenticated redirect, return navigation, and sign-out.
- [ ] 2.2 Register browser Refit clients against the gateway's BFF route with authorization limited to that origin, and trigger existing idempotent user provisioning after sign-in; verify Playwright confirms first-login provisioning and authenticated BFF requests without direct API calls.

## 3. Client State and Live Updates

- [ ] 3.1 Replace server `IHttpContextAccessor` token-handler registrations with WebAssembly-compatible authenticated HTTP clients for Refit and SSE; verify API calls and SSE requests carry authorization only to the gateway BFF route.
- [ ] 3.2 Replace circuit-bound order reconciliation cancellation with authenticated client-lifetime cancellation and adapt SSE streaming for browser transport; verify live order updates, reconnect behavior, sign-out cleanup, and the state-management unit tests.

## 4. Integration Verification

- [ ] 4.1 Update the integration-test factory and Playwright coverage to use the Aspire gateway URL; verify sign-in, user provisioning, ordering, checkout, and live status updates against the WASM client.
- [ ] 4.2 Normalize `openspec/specs/auth/user-provisioning/spec.md` from `## ADDED Requirements` to `## Requirements` when applying the delta; verify `openspec validate auth/user-provisioning --type spec --strict` passes.
- [ ] 4.3 Run `dotnet build --configuration Release --nologo`, `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo`, and `dotnet test sample-tests/Pizzeria.Tests.Integration/ --configuration Release --no-build --nologo`; verify all checks pass and no server-side interactive circuit is established.
