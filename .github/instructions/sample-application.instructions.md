---
applyTo: "sample/**/*"
---

# Sample Application

## Overview

The sample pizzeria application demonstrates Lewee framework usage with a multi-service architecture.

**Current State:**
- Pizzeria Store API (operational)
- Pizzeria Store Web (`Pizzeria.Store.Web` - Blazor WebAssembly front-end)
- PostgreSQL database (managed by Aspire)
- Authentication services (`Pizzeria.Auth`)
- Shared configuration (`Pizzeria.Configuration`)

## Running the Sample

**Quick Start:**
```bash
dotnet run --project ./sample/Pizzeria.AppHost/
```

**What Happens:**
1. .NET Aspire dashboard starts (typically at https://localhost:17268)
2. PostgreSQL container launches automatically
3. Pizzeria Store API becomes available
4. All services are monitored through the Aspire dashboard

**Access Points:**
- Aspire Dashboard: Check console output for URL (typically https://localhost:17268)
- Store API: URL shown in Aspire dashboard

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

## Architecture Notes

- **Orchestration**: .NET Aspire manages all services and containers
- **Database**: PostgreSQL with automatic schema management
- **No Manual Setup**: Aspire handles container lifecycle
- **Message Bus**: RabbitMQ planned for future inter-service communication
