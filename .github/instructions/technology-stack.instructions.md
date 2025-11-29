---
applyTo: "**/*"
---

# Technology Stack Reference

## Core Dependencies

| Technology | Version | Purpose | Documentation |
|------------|---------|---------|---------------|
| .NET | 10.0 | Runtime and SDK | [docs.microsoft.com](https://docs.microsoft.com/dotnet) |
| .NET Aspire | Latest | Service orchestration | [learn.microsoft.com/aspire](https://learn.microsoft.com/dotnet/aspire) |
| PostgreSQL | Latest | Primary database | [postgresql.org](https://postgresql.org) |
| Entity Framework Core | 10.0 | ORM | [docs.microsoft.com/ef](https://docs.microsoft.com/ef/core) |
| MediatR | 12.5.0 | Mediator pattern (free version) | [mediatr.io](https://mediatr.io) |
| FastEndpoints | Latest | API endpoints | [fast-endpoints.com](https://fast-endpoints.com) |
| FluentValidation | 8.7.0 | Validation (free version) | [fluentvalidation.net](https://fluentvalidation.net) |
| Ardalis.Specification | Latest | Specification pattern | [specification.ardalis.com](http://specification.ardalis.com) |
| xUnit | Latest | Testing framework | [xunit.net](https://xunit.net) |

## Package Version Strategy

**Pinned Versions:**
- MediatR 12.5.0 (last free version)
- FluentValidation 8.7.0 (last free version)

**Latest Versions:**
- All Microsoft packages (.NET, EF Core, Aspire)
- Supporting libraries (Npgsql, FastEndpoints, etc.)

**Rationale:** Balance between stability and staying current with .NET ecosystem
