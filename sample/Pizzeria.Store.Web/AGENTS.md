# Pizzeria Store Web Application

This is the Blazor Web App (interactive rendering) for the Pizzeria Store application, demonstrating the Lewee framework's Flux, Keycloak and Refit capabilities.

## Copilot Instructions

For development guidance specific to Blazor components in this project, see the [Blazor Instructions](../../.github/instructions/blazor.instructions.md).

## Key Patterns

### Code-Behind Pattern

All Razor components in this project use the code-behind pattern:
- Component markup is in `.razor` files
- Component logic is in corresponding `.razor.cs` files (partial classes)

### State Management

This project uses Fluxor for state management via `Pizzeria.Store.StateManagement` and `Pizzeria.Store.StateManagement.csproj`.

The state management is implemented using the Flux/Redux pattern, which allows for a unidirectional data flow and predictable state changes.

## Related Instructions

- [Sample Application Instructions](../../.github/instructions/sample-application.instructions.md)
- [Code Quality Instructions](../../.github/instructions/code-quality.instructions.md)
