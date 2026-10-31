# Pizzeria Ordering Web Client

This project is a standalone Blazor WebAssembly client served through the Aspire Blazor Gateway.
It uses browser-side Keycloak authentication, Fluxor state management, and the BFF as its only
backend entry point. Do not add server-side rendering, interactive circuits, or direct API service
addresses to this project.

## Copilot Instructions

- [Blazor Instructions](../../.github/instructions/blazor.instructions.md)
- [Sample Application Instructions](../../.github/instructions/sample-application.instructions.md)
- [Code Quality Instructions](../../.github/instructions/code-quality.instructions.md)

## Key Patterns

- Keep component logic in `.razor.cs` partial classes; avoid `@code` blocks.
- Configure browser HTTP clients against the gateway's BFF proxy route.
- Restrict bearer-token handlers to the gateway BFF URL.
- Preserve `X-Correlation-ID` propagation for order-command and SSE reconciliation.
- Use browser authentication state for user provisioning and live-update lifecycle.
- Aspire runs the client and gateway from `sample/Pizzeria.AppHost/`.
