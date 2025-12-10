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
| `Lewee.Domain` | Domain layer abstractions including base classes for entities, aggregate roots, value objects, domain events, specifications, and repository interfaces |
| `Lewee.Application` | Application layer with CQRS implementation using MediatR, including commands, queries, pipeline behaviors for logging, validation, and exception handling |
| `Lewee.Shared` | Cross-cutting utilities including logging constants, HTTP context constants, request headers, and enum extension methods |
| `Lewee.Contracts` | Shared contracts for client-server communication including client messages, event channels, and Fluxor state management action interfaces |
| `Lewee.Infrastructure.Data` | Entity Framework Core integration with repository implementation, domain event dispatching, audit interceptors, and database configuration |
| `Lewee.Infrastructure.PostgreSQL` | PostgreSQL-specific database configuration and setup using Npgsql with schema support and migration configuration |
| `Lewee.Infrastructure.AspNet` | ASP.NET Core integration including authenticated user services, SignalR hub configuration, and correlation ID middleware |
| `Lewee.Infrastructure.AspNet.WebApi` | FastEndpoints-based command and query endpoint base classes with MediatR integration and result handling |
| `Lewee.Blazor` | Blazor integration with SignalR message receiving, HTTP correlation ID handling, and server health monitoring |
| `Lewee.StateManagement` | Fluxor state management configuration and base classes for request/query state with reducer extension methods |
| `Lewee.Playwright` | Playwright browser automation utilities for integration testing with page wrapper and extension methods |

## Key Requirements

- **XML Documentation**: All public and protected APIs must have XML documentation comments
- **Code Coverage**: All changes must have at least 90% line coverage
- **Backward Compatibility**: Maintain backward compatibility for all public APIs

## Build Commands

```bash
# Build framework packages
dotnet build --configuration Release --nologo

# Run framework tests
dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo
