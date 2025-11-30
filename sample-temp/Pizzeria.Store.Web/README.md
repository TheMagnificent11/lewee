# Pizzeria Store Web Application

This is the Blazor WebAssembly front-end for the Pizzeria Store application, demonstrating the Lewee framework's Blazor integration capabilities.

## Copilot Instructions

For development guidance specific to Blazor components in this project, see the [Blazor Instructions](../../.github/instructions/blazor.instructions.md).

## Key Patterns

### Code-Behind Pattern

All Razor components in this project use the code-behind pattern:
- Component markup is in `.razor` files
- Component logic is in corresponding `.razor.cs` files (partial classes)

### Example

See `Pages/Home.razor` and `Pages/Home.razor.cs` for a complete example of this pattern.

### State Management

This project uses Fluxor for state management:
- States are defined in the `States/` directory
- Actions and reducers follow Fluxor conventions
- Components inherit from `FluxorComponent`

## Related Instructions

- [Sample Application Instructions](../../.github/instructions/sample-application.instructions.md)
- [Code Quality Instructions](../../.github/instructions/code-quality.instructions.md)
