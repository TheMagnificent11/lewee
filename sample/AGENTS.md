# Pizzeria Sample Application

This directory contains the sample Pizzeria application that demonstrates Lewee framework usage patterns.

## Copilot Instructions

For development guidance in this directory, see the following instruction files:

- [Sample Application Instructions](../.github/instructions/sample-application.instructions.md) - Running and developing the sample
- [Blazor Instructions](../.github/instructions/blazor.instructions.md) - Blazor component development (for `Pizzeria.Ordering.Web`)
- [Code Quality Instructions](../.github/instructions/code-quality.instructions.md) - Coding standards

## Projects

| Project | Description |
| --------- | ------------- |
| `Pizzeria.AppHost` | .NET Aspire orchestration entry point |
| `Pizzeria.ServiceDefaults` | Shared Aspire configurations |
| `Pizzeria.Common` | Shared utilities and constants |
| `Pizzeria.Auth` | Authentication configuration |
| `Pizzeria.Configuration` | Application configuration |
| `Pizzeria.DataSeeder` | Database seeding utilities |
| `Pizzeria.Store.Domain` | Domain models and business logic |
| `Pizzeria.Store.Application` | CQRS commands/queries |
| `Pizzeria.Store.Data` | EF Core DbContext and migrations |
| `Pizzeria.Store.Contracts` | API DTOs and contracts |
| `Pizzeria.Ordering.StateManagement` | Fluxor state management features |
| `Pizzeria.Ordering.Components` | Shared Blazor components |
| `Pizzeria.Store.Api` | Web API with FastEndpoints for CQRS commands/queries |
| `Pizzeria.Ordering.Web` | Standalone Blazor WebAssembly client served through the Aspire Blazor Gateway |

## Architecture

The sample application is split into two main runtime projects:

### Pizzeria.Store.Api

- **Purpose**: Backend API using FastEndpoints for CQRS commands/queries
- **Authentication**: Keycloak JWT Bearer tokens
- **Features**:
  - Database access via Entity Framework Core
  - Domain event handling
  - Server-Sent Events (SSE) for real-time notifications
  
### Pizzeria.Ordering.Web

- **Purpose**: Standalone Blazor WebAssembly client served by the Aspire Blazor Gateway
- **Authentication**: Browser-side Keycloak authorization-code flow with PKCE
- **Features**:
  - Refit HTTP client to call the BFF through the gateway
  - Fluxor state management
  - Authenticated SSE client to receive real-time events through the BFF

## Running the Sample

```bash
dotnet run --project ./sample/Pizzeria.AppHost/
```

This will start:

1. .NET Aspire dashboard (check console output for URL)
2. PostgreSQL container (managed by Aspire)
3. Keycloak authentication server
4. Configuration service
5. Pizzeria Store API
6. Aspire Blazor Gateway serving the Pizzeria Store Web client

## Key Patterns Demonstrated

- Domain-driven design architecture
- CQRS with MediatR (API side)
- Entity Framework with PostgreSQL
- FastEndpoints API
- Standalone Blazor WebAssembly client with Fluxor state management
- Refit HTTP client for API communication
- Server-Sent Events for real-time notifications
- .NET Aspire orchestration

## Documentation

- **No XML documentation comments** in sample code (use inline comments instead)
- Use comments to explain patterns and demonstrate best practices
