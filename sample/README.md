# Pizzeria Sample Application

This directory contains the sample Pizzeria application that demonstrates Lewee framework usage patterns.

## Copilot Instructions

For development guidance in this directory, see the following instruction files:

- [Sample Application Instructions](../.github/instructions/sample-application.instructions.md) - Running and developing the sample
- [Blazor Instructions](../.github/instructions/blazor.instructions.md) - Blazor component development (for `Pizzeria.Store.Web`)
- [Code Quality Instructions](../.github/instructions/code-quality.instructions.md) - Coding standards

## Projects

| Project | Description |
|---------|-------------|
| `Pizzeria.AppHost` | .NET Aspire orchestration entry point |
| `Pizzeria.ServiceDefaults` | Shared Aspire configurations |
| `Pizzeria.Common` | Shared utilities and constants |
| `Pizzeria.Auth` | Authentication configuration |
| `Pizzeria.Configuration` | Application configuration |
| `Pizzeria.Store.Domain` | Domain models and business logic |
| `Pizzeria.Store.Application` | CQRS commands/queries |
| `Pizzeria.Store.Data` | EF Core DbContext and migrations |
| `Pizzeria.Store.Contracts` | API DTOs and contracts |
| `Pizzeria.Store.Api` | FastEndpoints Web API |
| `Pizzeria.Store.Web` | Blazor WebAssembly front-end |

## Running the Sample

```bash
dotnet run --project ./sample/Pizzeria.AppHost/
```

This will start:
1. .NET Aspire dashboard (check console output for URL)
2. PostgreSQL container (managed by Aspire)
3. Pizzeria Store API
4. Pizzeria Store Web application

## Key Patterns Demonstrated

- Domain-driven design architecture
- CQRS with MediatR
- Entity Framework with PostgreSQL
- FastEndpoints API
- Blazor WebAssembly with Fluxor state management
- .NET Aspire orchestration

## Documentation

- **No XML documentation comments** in sample code (use inline comments instead)
- Use comments to explain patterns and demonstrate best practices
