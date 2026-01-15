---
applyTo: "**/*"
---

# Repository Structure

## Project Organization

```
lewee/
│
├── src/                          # Framework packages (core product)
│   ├── Directory.Build.props     # Framework-specific build properties
│   ├── Lewee.Domain/             # Domain layer abstractions and base classes
│   ├── Lewee.Application/        # Application layer with CQRS and validation
│   ├── Lewee.Common/             # Cross-cutting utilities, result types, and constants
│   ├── Lewee.Infrastructure.Data/           # Entity Framework core integration
│   ├── Lewee.Infrastructure.PostgreSQL/     # PostgreSQL-specific features
│   ├── Lewee.Infrastructure.Auth/           # Authentication infrastructure
│   ├── Lewee.Infrastructure.Correlate/      # Correlation ID middleware
│   ├── Lewee.Infrastructure.FastEndpoints/  # FastEndpoints base classes
│   ├── Lewee.Infrastructure.Fluxor/         # Fluxor state management and SSE client
│   ├── Lewee.Infrastructure.ServerEvents/   # Server-Sent Events infrastructure
│   ├── Lewee.Infrastructure.Keycloak/       # Keycloak authentication
│   ├── Lewee.Infrastructure.Refit/          # Refit HTTP client integration
│   └── Lewee.Playwright/         # Playwright browser automation for testing
│
├── sample/                       # Example application (demonstrates usage)
│   ├── Directory.Build.props     # Sample-specific build properties
│   ├── .editorconfig             # Sample-specific analyzer rules
│   ├── Pizzeria.AppHost/         # .NET Aspire orchestration entry point
│   ├── Pizzeria.ServiceDefaults/ # Shared Aspire configurations
│   ├── Pizzeria.Common/          # Shared utilities and constants
│   ├── Pizzeria.Store.Domain/    # Domain models and business logic
│   ├── Pizzeria.Store.Application/  # CQRS commands/queries
│   ├── Pizzeria.Store.Data/      # EF Core DbContext and migrations
│   ├── Pizzeria.Store.Contracts/ # API DTOs and contracts
│   ├── Pizzeria.Store.StateManagement/ # Fluxor state management
│   ├── Pizzeria.Store.Components/ # Shared Blazor components
│   ├── Pizzeria.Store.Api/       # FastEndpoints Web API
│   ├── Pizzeria.Store.Web/       # Blazor Server front-end
│   ├── Pizzeria.Store/           # Pizzeria Store shared utilities
│   ├── Pizzeria.Auth/            # Authentication services
│   ├── Pizzeria.Configuration/   # Shared configuration
│   └── Pizzeria.DataSeeder/      # Database seeding utilities
│
├── tests/                        # Framework unit tests
│   ├── Directory.Build.props     # Test-specific build properties
│   ├── .editorconfig             # Test-specific analyzer rules
│   └── [Project].Tests.Unit/
│
├── sample-tests/                 # Sample application tests
│   ├── Directory.Build.props     # Sample test-specific build properties
│   ├── .editorconfig             # Sample test-specific analyzer rules
│   ├── Pizzeria.Tests.Integration/  # End-to-end integration tests
│   └── Pizzeria.Store.Domain.Tests/ # Domain unit tests
│
├── Directory.Build.props         # Root build properties (all projects)
├── Directory.Packages.props      # Central Package Management
├── Tests.props                   # Shared test configuration
└── .editorconfig                 # Root code style rules
```

## Key Architecture Layers

| Layer | Purpose | Example Projects | Dependencies Flow |
|-------|---------|-----------------|-------------------|
| Common | Shared contracts and utilities | Lewee.Common | No dependencies on other Lewee layers |
| Domain | Business logic and entities | Lewee.Domain, Pizzeria.Store.Domain | Depends on Common |
| Application | Use cases and orchestration | Lewee.Application, Pizzeria.Store.Application | Depends on Domain |
| Infrastructure (Server) | Server-side concerns (DB, API, Auth) | Lewee.Infrastructure.Data, Lewee.Infrastructure.ServerEvents | Depends on Application and Domain |
| Infrastructure (Client) | Client-side concerns (State, SSE) | Lewee.Infrastructure.Fluxor | Depends on Common |
| Presentation | User interface | Pizzeria.Store.Api, Pizzeria.Store.Web | Depends on all layers |

## Configuration Files

| File | Purpose | When to Edit |
|------|---------|-------------|
| `lewee.slnx` | Solution with all projects | Adding/removing projects |
| `Directory.Build.props` | Global MSBuild properties (targets .NET 10.0) | Changing global build settings |
| `src/Directory.Build.props` | Framework package properties | Changing framework-specific settings |
| `tests/Directory.Build.props` | Test project properties | Changing test-specific settings |
| `sample/Directory.Build.props` | Sample application properties | Changing sample-specific settings |
| `sample-tests/Directory.Build.props` | Sample test properties | Changing sample test-specific settings |
| `Directory.Packages.props` | Central Package Management (CPM) | Adding/updating NuGet packages |
| `Tests.props` | Shared test configuration | Changing test framework or packages |
| `.editorconfig` | Global code style rules | Adjusting global code formatting |
| `tests/.editorconfig` | Test-specific analyzer rules | Adjusting test-specific rules |
| `sample/.editorconfig` | Sample-specific analyzer rules | Adjusting sample-specific rules |
| `sample-tests/.editorconfig` | Sample test-specific analyzer rules | Adjusting sample test-specific rules |
| `sample/Pizzeria.AppHost/Program.cs` | Aspire orchestration | Configuring services |
| `.github/workflows/ci.yml` | CI/CD pipeline | Modifying build/test process |

## Common Development Tasks

| Task | Command | Use Case |
|------|---------|----------|
| Clean artifacts | `dotnet clean` | Remove build outputs |
| Full rebuild | `dotnet build --configuration Release --no-incremental` | After major changes |
| Run specific tests | `dotnet test tests/Lewee.Domain.Tests.Unit/` | Test single project |
| Run sample app | `dotnet run --project sample/Pizzeria.AppHost/` | Manual testing |
| Check outdated packages | `dotnet list package --outdated` | Dependency updates |
| Format code | `dotnet format` | Fix style issues |
| Create packages | `dotnet pack --configuration Release --nologo` | Prepare for release |
