---
applyTo: "**/*.cs"
---

# Development Focus Areas

## Framework Packages (src/Lewee.*)

**Domain Layer (Lewee.Domain)**
- Core business logic abstractions
- Base classes for entities, value objects, aggregates
- Domain events and specifications
- **Key Pattern:** Rich domain models with encapsulated business rules

**Application Layer (Lewee.Application)**
- CQRS implementation with MediatR
- FluentValidation integration
- Pipeline behaviors (logging, validation, correlation)
- **Key Pattern:** Thin application services orchestrating domain logic

**Infrastructure Layer (Lewee.Infrastructure.*)**
- Entity Framework Core integration
- PostgreSQL-specific optimizations
- ASP.NET Core middleware and extensions
- Blazor components and utilities
- **Key Pattern:** Adapters implementing domain interfaces

**Shared Utilities (Lewee.Common)**
- Cross-cutting concerns
- Result types (CommandResult, QueryResult)
- Client messaging contracts
- Logging constants
- HTTP headers
- Extension methods
- **Key Pattern:** Minimal-dependency utilities

## Sample Application (sample/Pizzeria.*)

**Purpose:** Demonstrates framework usage patterns and best practices

**Key Demonstrations:**
- Domain-driven design architecture
- CQRS with MediatR
- Entity Framework with PostgreSQL
- FastEndpoints API
- .NET Aspire orchestration

**Learning Resources:**
- Domain models: `sample/Pizzeria.Store.Domain/`
- CQRS handlers: `sample/Pizzeria.Store.Application/`
- API endpoints: `sample/Pizzeria.Store.Api/`
- Database configuration: `sample/Pizzeria.Store.Data/`

## Contribution Guidelines

**When working on framework (Lewee.*):**
1. Maintain backward compatibility
2. Add XML documentation for public and protected APIs
3. Follow existing architectural patterns
4. Add unit tests for new functionality
5. Ensure at least 90% line coverage for all changes
6. Update relevant README.md files

**When working on sample app (Pizzeria.*):**
1. Demonstrate best practices
2. Keep examples clear and focused
3. Update comments to explain patterns (use inline comments, not XML documentation)
4. Do not add XML documentation comments (///) to sample app code
5. Ensure integration tests pass

**Code Review Checklist:**
- [ ] Follows domain-driven design principles
- [ ] Maintains clean architecture boundaries
- [ ] Includes appropriate tests
- [ ] Framework changes have at least 90% line coverage
- [ ] Documentation updated
- [ ] No warnings or style violations
