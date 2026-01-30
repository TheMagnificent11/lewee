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
| `Lewee.Common` | Cross-cutting utilities including result types (CommandResult, QueryResult), client messaging contracts, logging constants, HTTP context constants, request headers, and enum extension methods |
| `Lewee.Infrastructure.Data` | Entity Framework Core integration with repository implementation, domain event dispatching, audit interceptors, and database configuration |
| `Lewee.Infrastructure.PostgreSQL` | PostgreSQL-specific database configuration and setup using Npgsql with schema support and migration configuration |
| `Lewee.Infrastructure.AspNet` | ASP.NET Core integration including authenticated user services and correlation ID middleware |
| `Lewee.Infrastructure.Auth` | Authentication infrastructure with authenticated user service implementation and HttpContext integration |
| `Lewee.Infrastructure.Correlate` | Correlation ID infrastructure using the Correlate library for distributed request tracing |
| `Lewee.Infrastructure.FastEndpoints` | FastEndpoints-based command and query endpoint base classes with MediatR integration and result handling |
| `Lewee.Infrastructure.Fluxor` | Fluxor state management infrastructure with base state classes, reducer extensions, effect base classes, and action interfaces for Blazor applications |
| `Lewee.Infrastructure.ServerEvents` | Server-Sent Events infrastructure for real-time client event broadcasting with SSE endpoint configuration and client event receiver |
| `Lewee.Infrastructure.Keycloak` | Keycloak OpenID Connect authentication integration with .NET Aspire service discovery, cookie authentication, and customizable authentication events |
| `Lewee.Infrastructure.Refit` | Refit HTTP client infrastructure with authentication token propagation and correlation ID support for domain-driven design |
| `Lewee.Blazor` | Blazor client-side infrastructure with Fluxor state management, HTTP correlation ID handling, and server health monitoring |
| `Lewee.Playwright` | Playwright browser automation utilities for integration testing with page wrapper and FluentAssertions extensions |

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
