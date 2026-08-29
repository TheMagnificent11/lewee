---
applyTo: "sample/**/*"
---

# Sample Application

## Overview

The sample pizzeria application demonstrates Lewee framework usage with a multi-service architecture.

**Current State:**
- Pizzeria BFF (`Pizzeria.Bff` - YARP reverse proxy used by the web frontend)
- Pizzeria Auth API (`Pizzeria.Auth.Api` - reusable auth endpoints)
- Pizzeria Store API (`Pizzeria.Store.Api` - FastEndpoints Web API)
- Pizzeria Store Web (`Pizzeria.Store.Web` - Blazor Web App with Interactive Server)
- PostgreSQL database (managed by Aspire)
- Keycloak authentication server (managed by Aspire)
- Authentication services (`Pizzeria.Auth`)
- Database migration/seeding console app (`Pizzeria.Configuration`), run once at startup via Aspire's experimental `AddCSharpApp` API

## Running the Sample

**Quick Start:**
```bash
dotnet run --project ./sample/Pizzeria.AppHost/
```

**What Happens:**
1. .NET Aspire dashboard starts (typically at https://localhost:17268)
2. PostgreSQL container launches automatically
3. Pizzeria Auth API and Store API become available
4. Pizzeria BFF becomes available and routes requests to the APIs
5. Pizzeria Store Web becomes available
6. All services are monitored through the Aspire dashboard

**Access Points:**
- Aspire Dashboard: Check console output for URL (typically https://localhost:17268)
- Store API: URL shown in Aspire dashboard
- Auth API: URL shown in Aspire dashboard
- BFF: URL shown in Aspire dashboard
- Store Web: URL shown in Aspire dashboard

## Development Workflow

**For Framework Changes:**
```bash
# Make changes to Lewee.* projects
dotnet build --configuration Release --nologo
dotnet test --configuration Release --no-build --nologo
```

**For Sample App Changes:**
```bash
# Make changes to Pizzeria.* projects
dotnet run --project ./sample/Pizzeria.AppHost/
# Validate in Aspire dashboard
```

**For Testing Integration:**
```bash
# Use integration tests (preferred method)
dotnet test sample-tests/Pizzeria.Tests.Integration/
```

## Code Quality

Follow [code quality instructions](./code-quality.instructions.md), especially for logging, dependency injection and ASP.NET application standards.

## Code Style

No need for XML documentation in sample application code. Follow existing patterns and maintain readability.

## Namespace Conventions

**Feature Namespaces:** Use feature-based namespaces (organized by aggregate root) instead of type-based namespaces.

**Correct:**
```
Pizzeria.Store.Api.Orders
Pizzeria.Store.Api.Pizzas
Pizzeria.Store.Api.Customers
```

**Incorrect:**
```
Pizzeria.Store.Api.Endpoints
```

See `Pizzeria.Store.Application` project for guidance on feature namespace organization.

## Architecture Notes

- **Orchestration**: .NET Aspire manages all services and containers
- **Database**: PostgreSQL with automatic schema management
- **Database Migration/Seeding**: `Pizzeria.Configuration` is a plain C# console app added to the AppHost via the experimental `AddCSharpApp` API (requires suppressing diagnostic `ASPIRECSHARPAPPS001`). It runs migrations/seeding once and exits; the Pizza Store API uses `WaitForCompletion` to wait for it to finish before starting.
- **Authentication**: Keycloak for OpenID Connect authentication
- **No Manual Setup**: Aspire handles container lifecycle
- **API**: FastEndpoints for CQRS commands/queries
- **BFF**: YARP reverse proxy; the web frontend accesses backend APIs only through this service
- **Web**: Blazor Web App with Interactive Server render mode
- **State Management**: Fluxor for client-side state management
- **Real-time Updates**: Server-Sent Events for real-time notifications
- **Message Bus**: RabbitMQ planned for future inter-service communication
