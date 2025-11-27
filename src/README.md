# Lewee Framework Packages

This directory contains the core Lewee framework packages that provide domain-driven design infrastructure for ASP.NET applications.

## Copilot Instructions

For development guidance in this directory, see the following instruction files:

- [Code Quality Instructions](../.github/instructions/code-quality.instructions.md) - Enforcement rules and coding standards
- [Development Focus Instructions](../.github/instructions/development-focus.instructions.md) - Framework development guidelines
- [Validation Workflows](../.github/instructions/validation-workflows.instructions.md) - Required validation after changes

## Packages

| Package | Description |
|---------|-------------|
| `Lewee.Domain` | Domain layer abstractions, base classes for entities, value objects, and aggregates |
| `Lewee.Application` | Application layer with CQRS implementation using MediatR |
| `Lewee.Shared` | Cross-cutting utilities and constants |
| `Lewee.Contracts` | API contract definitions |
| `Lewee.Infrastructure.Data` | Entity Framework Core integration |
| `Lewee.Infrastructure.PostgreSQL` | PostgreSQL-specific features |
| `Lewee.Infrastructure.AspNet` | ASP.NET Core integration and middleware |
| `Lewee.Infrastructure.AspNet.WebApi` | Web API utilities |
| `Lewee.Blazor` | Blazor component library |

## Key Requirements

- **XML Documentation**: All public and protected APIs must have XML documentation comments
- **Code Coverage**: All changes must have at least 90% line coverage
- **Backward Compatibility**: Maintain backward compatibility for all public APIs

## Build Commands

```bash
# Build framework packages
dotnet build lewee.sln --configuration Release --nologo

# Run framework tests
dotnet test lewee.sln --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo
```
