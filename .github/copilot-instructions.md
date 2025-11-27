# Lewee Development Instructions

## Overview

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.NET. This repository contains both the Lewee framework packages and a sample restaurant management application demonstrating their usage.

## How to Use These Instructions

**Priority Order:**
1. First, consult these instructions for repository-specific guidance
2. Then, use search or bash commands only when you encounter unexpected information that does not match the info here

**Success Criteria:** Your changes should:
- Build without errors or warnings
- Pass all existing tests
- Follow the established coding patterns
- Maintain backward compatibility for framework packages
- Be minimal and focused

## Visual Studio Solution

**Do not use solution folders.**

There are directory folders for the `src`, `tests`, `sample` and `sample-tests` projects.

However, the C# projects are named so that test projects appear next to their corresponding source projects in the Visual Studio Solution Explorer.

**Solution File:** The repository uses `lewee.slnx`.

## Environment Setup

### Prerequisites (Required)

| Component | Version | Installation Command | Verification |
|-----------|---------|---------------------|--------------|
| .NET SDK | 10.0+ | `curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh \| bash /dev/stdin --channel 10.0 --install-dir ~/.dotnet` | `dotnet --version` |
| .NET Aspire | Latest | `dotnet workload install aspire` | `dotnet workload list` |
| Docker Desktop | Latest | Platform-specific | `docker --version` |

**PATH Configuration:**
```bash
export PATH="~/.dotnet:$PATH"
```

**Critical:** The repository targets .NET 10.0 and will not build with older versions.

## Build and Test Commands

### Command Reference

| Task | Command | Typical Duration | Timeout Setting | Notes |
|------|---------|------------------|-----------------|-------|
| Clean | `dotnet clean lewee.sln` | ~2s | 60s | Safe to run anytime |
| Restore | `dotnet restore lewee.sln --nologo` | 2-30s | 120s | Depends on cache state |
| Build | `dotnet build lewee.sln --configuration Release --no-restore --nologo` | 12-20s | 120s | **NEVER CANCEL** |
| Full Rebuild | `dotnet build lewee.sln --configuration Release --no-incremental --nologo` | ~12s | 120s | **NEVER CANCEL** |
| Unit Tests | `dotnet test lewee.sln --configuration Release --no-build --nologo` | ~4s | 60s | Fast validation |
| Integration Tests | `dotnet test lewee.sln --configuration Release --no-build --nologo` | 300+s | 600s | Uses Aspire containers - **NEVER CANCEL** |
| Pack | `dotnet pack lewee.sln --configuration Release --nologo --no-build` | ~2s | 60s | Creates NuGet packages |

### Critical Rules

**NEVER:**
- Cancel builds or long-running commands before completion
- Use timeouts less than the recommended values
- Run integration tests without Docker Desktop running

**ALWAYS:**
- Wait for command completion
- Use `--nologo` flag to reduce output noise
- Run tests after making code changes

## Sample Application

### Overview
The sample pizzeria application demonstrates Lewee framework usage with a multi-service architecture.

**Current State:**
- Pizzeria Store API (operational)
- PostgreSQL database (managed by Aspire)

**Future Roadmap:**
- Pizzeria Kitchen service
- Pizzeria Delivery service
- RabbitMQ message bus integration

### Running the Sample

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

### Development Workflow

**For Framework Changes:**
```bash
# Make changes to Lewee.* projects
dotnet build lewee.sln --configuration Release --nologo
dotnet test lewee.sln --configuration Release --no-build --nologo
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

### Architecture Notes

- **Orchestration**: .NET Aspire manages all services and containers
- **Database**: PostgreSQL with automatic schema management
- **No Manual Setup**: Aspire handles container lifecycle
- **Message Bus**: RabbitMQ planned for future inter-service communication

## Code Quality Standards

### Enforcement Rules

| Rule | Status | Impact |
|------|--------|--------|
| Warnings as Errors | Enabled | Build fails on any warning |
| Code Style in Build | Enforced | Style violations break the build |
| Documentation Generation | Required | All framework projects must generate XML docs |
| Code Coverage | Required for Framework | Pull requests with changes to `src/` directory (Lewee packages) must have at least 90% line coverage |

### Dependency Management

The solution uses Central Package Management via `Directory.Packages.props`.

Do not unnecessarily add package and project references; use implicit references where possible.

Therefore, always check for existing references in packages and projects that are already referenced implicitly in a C# project before adding new ones.

Furthermore, when working on application C# projects like web applications, do not add a reference if it comes in the `Microsoft.NET.Sdk.Web` web SDK.

**Exception:** During major framework upgrades (e.g., .NET 9 to .NET 10), explicit package references may be temporarily required to resolve version conflicts with third-party packages that have hard upper-bound constraints. These should be documented and removed once the third-party packages are updated for the new framework version.

### Project File Configuration

**IMPORTANT: Do NOT add build properties to individual `.csproj` files.**

All build configuration is centrally managed through a hierarchy of configuration files to ensure consistency across the solution.

#### Configuration File Hierarchy

**Root Configuration Files:**
- `Directory.Build.props` - Global MSBuild properties applied to all projects
- `Tests.props` - Shared test project configuration
- `.editorconfig` - Global code style rules and analyzer settings

**Directory-Specific Configuration:**
- `src/Directory.Build.props` - Framework package-specific properties (inherits from root)
- `tests/Directory.Build.props` - Test project properties (inherits from root and Tests.props)
- `sample/Directory.Build.props` - Sample application properties (inherits from root)
- `sample-tests/Directory.Build.props` - Sample test properties (inherits from root and Tests.props)
- `tests/.editorconfig` - Test-specific analyzer rules (inherits from root)
- `sample/.editorconfig` - Sample-specific analyzer rules (inherits from root)
- `sample-tests/.editorconfig` - Sample test-specific analyzer rules (inherits from root)

**Root `Directory.Build.props` Contains:**
- Target framework (.NET 10.0) and language version
- Global build settings (warnings as errors, code style enforcement)
- Repository metadata (URL, authors, etc.)
- Analyzer package references (Meziantou, SonarAnalyzer, StyleCop, etc.)

**`Tests.props` Contains:**
- Test framework package references (xUnit, FluentAssertions, etc.)
- Test-specific property settings
- Code coverage exclusion attributes

**Directory-Specific `Directory.Build.props` Files:**
- `src/` - Package generation, XML documentation, symbol packages, nullable reference types
- `tests/` - Imports Tests.props for test-specific configuration
- `sample/` - Nullable reference types, warning suppressions, code coverage exclusion
- `sample-tests/` - Imports Tests.props for test-specific configuration

**Directory-Specific `.editorconfig` Files:**
- `tests/` - CA1707 suppression for underscores in test method names
- `sample/` - SA1313 suppressions for Effects and Reducer parameter naming
- `sample-tests/` - CA1707 suppression for underscores in sample test method names

**Never add these properties to individual project files:**
- `<GenerateDocumentationFile>`
- `<NoWarn>`
- `<TreatWarningsAsErrors>`
- `<AnalysisLevel>` / `<AnalysisMode>`
- `<Nullable>`
- `<IsPackable>`
- Any other build/analyzer configuration

**If you need project-specific settings:**
1. First check if the appropriate directory-level `Directory.Build.props` already provides the correct behavior
2. If the setting should apply to all projects in a directory, add it to that directory's `Directory.Build.props`
3. If truly project-specific, discuss with the repository owner before adding

**Why this matters:**
- Ensures consistent build behavior across all projects
- Makes it easier to update settings globally or by directory
- Prevents configuration drift between projects
- Reduces maintenance burden
- Provides clear separation between framework, test, and sample configurations

### Coding Style

**Format Command:**
```bash
dotnet format lewee.sln
```

**Configuration:**
- Defined in `.editorconfig` (root and directory-specific)
- Enforced during build
- Must be applied before committing

**Quality Checklist:**
- [ ] No compiler warnings
- [ ] No style violations
- [ ] No unused usings or variables
- [ ] No magic strings or numbers, use constants or enums
- [ ] Address compiler information messages that result for Roslyn analyzers
- [ ] XML documentation for public and protected APIs **only** for C# projects within the `src` directory (Lewee framework packages)
- [ ] No XML documentation for sample application code (`sample/` directory)
- [ ] Follows existing patterns in the codebase
- [ ] Framework changes (`src/` directory) have at least 90% line coverage
- [ ] No magic strings for Playwright/bUnit selectors; expose a constant from the component and use that instead

**Blazor Components:**
- Use code-behind pattern with partial classes for Razor components
- Create a separate `.razor.cs` file for component logic
- Do not add `@code` blocks directly in `.razor` files, use code-behind instead (partial classes named `[ComponentName].razor.cs`)
  - All other `@` directives (e.g. ``@attribute`, `@inherits `@inject`, `@using`, etc.) should remain in the `.razor` file
- See `sample/Pizzeria.Store.Web/Pages/Home.razor` and `Home.razor.cs` for examples

### Logging

- Use logging scopes where possible to provide context (as opposed to structured properties within a log message
  - Prefer to inherit structured properties from the scope when they are passed in as parameters
    - Values like CorrelationId, TenantId, UserId etc that are passed in as method parameters should be added to the logging scope at the entry point of the request
- Do not use emojis in log messages

### bUnit/Playwright Testing Standards

- Do not use magic strings for selectors
  - Expose a constant from the component and use that instead

## Validation Workflows

### Required Validation After Changes

**Decision Tree:**
```
Did you change framework code (Lewee.*)?
├─ YES → Run all 4 validation workflows below
└─ NO → Did you change sample app (Pizzeria.*)?
    ├─ YES → Run workflows 1, 2, and 3
    └─ NO → Did you only change documentation?
        ├─ YES → Run workflow 1 only
        └─ NO → Run workflow 1 to be safe
```

### Workflow 1: Framework Build Validation

**When:** After any framework (Lewee.*) changes

**Commands:**
```bash
dotnet build lewee.sln --configuration Release --nologo
```

**Success Criteria:**
- All Lewee.* projects compile successfully
- Zero compilation warnings
- Zero style violations

### Workflow 2: Unit Test Validation

**When:** After code changes (not documentation-only)

**Commands:**
```bash
dotnet test lewee.sln --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo
```

**Success Criteria:**
- All unit tests pass
- No test failures or exceptions
- Test execution time < 30 seconds

### Workflow 3: Integration Test Validation

**When:** After infrastructure or data layer changes

**Commands:**
```bash
dotnet test lewee.sln --filter "FullyQualifiedName~Integration" --configuration Release --no-build --nologo
```

**Prerequisites:**
- Docker Desktop running
- .NET Aspire workload installed

**Success Criteria:**
- All integration tests pass
- Database operations work correctly
- API endpoints respond as expected

**Note:** Aspire manages PostgreSQL test containers automatically

### Workflow 4: Package Validation

**When:** Before releasing framework updates

**Commands:**
```bash
dotnet pack lewee.sln --configuration Release --nologo --no-build
```

**Success Criteria:**
- NuGet packages created without errors
- Package versions are correct
- All dependencies properly referenced

## Repository Structure

### Project Organization

```
lewee/
│
├── src/                          # Framework packages (core product)
│   ├── Directory.Build.props     # Framework-specific build properties
│   ├── Lewee.Domain/             # Domain layer abstractions and base classes
│   ├── Lewee.Application/        # Application layer with CQRS and validation
│   ├── Lewee.Shared/             # Cross-cutting utilities and constants
│   ├── Lewee.Contracts/          # API contract definitions
│   ├── Lewee.Infrastructure.Data/           # Entity Framework core integration
│   ├── Lewee.Infrastructure.PostgreSQL/     # PostgreSQL-specific features
│   ├── Lewee.Infrastructure.AspNet/         # ASP.NET Core integration
│   ├── Lewee.Infrastructure.AspNet.WebApi/  # Web API utilities
│   └── Lewee.Blazor/             # Blazor component library
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
│   └── Pizzeria.Store.Api/       # FastEndpoints Web API
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

### Key Architecture Layers

| Layer | Purpose | Example Projects | Dependencies Flow |
|-------|---------|-----------------|-------------------|
| Domain | Business logic and entities | Lewee.Domain, Pizzeria.Store.Domain | No dependencies on other layers |
| Application | Use cases and orchestration | Lewee.Application, Pizzeria.Store.Application | Depends on Domain |
| Infrastructure | External concerns (DB, API) | Lewee.Infrastructure.*, Pizzeria.Store.Data | Depends on Application and Domain |
| Presentation | User interface | Pizzeria.Store.Api, Lewee.Blazor | Depends on all layers |

### Configuration Files

| File | Purpose | When to Edit |
|------|---------|-------------|
| `lewee.sln` | Solution with all projects | Adding/removing projects |
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

### Common Development Tasks

| Task | Command | Use Case |
|------|---------|----------|
| Clean artifacts | `dotnet clean lewee.sln` | Remove build outputs |
| Full rebuild | `dotnet build lewee.sln --configuration Release --no-incremental` | After major changes |
| Run specific tests | `dotnet test tests/Lewee.Domain.Tests.Unit/` | Test single project |
| Run sample app | `dotnet run --project sample/Pizzeria.AppHost/` | Manual testing |
| Check outdated packages | `dotnet list package --outdated` | Dependency updates |
| Format code | `dotnet format lewee.sln` | Fix style issues |
| Create packages | `dotnet pack lewee.sln --configuration Release --nologo` | Prepare for release |

## Troubleshooting Guide

### Problem Resolution Matrix

| Symptom | Likely Cause | Solution |
|---------|-------------|----------|
| Build fails with "NETSDK1045" | Wrong .NET version | Install .NET 10.0 SDK (see Environment Setup) |
| Integration tests fail to start | Aspire workload missing | `dotnet workload install aspire` |
| Aspire services won't start | Port conflicts | Check port availability, restart Docker |
| Unexplained build errors | Stale build artifacts | `dotnet clean lewee.sln` |
| Slow package restore | First run after clone | Normal - packages downloading from NuGet |
| Container startup failures | Docker not running | Start Docker Desktop |
| Test timeouts | Containers still starting | Wait longer, increase timeout |

### Debug Checklist

**Before asking for help:**
1. [ ] Verified .NET 10.0 SDK is installed (`dotnet --version`)
2. [ ] Ran `dotnet clean lewee.sln`
3. [ ] Checked Docker Desktop is running (for integration tests)
4. [ ] Reviewed error message carefully
5. [ ] Tried the solution from the matrix above

### Common Error Patterns

**Build Errors:**
```
NETSDK1045: The current .NET SDK does not support targeting .NET 10.0
→ Solution: Install .NET 10.0 SDK
```

**Test Errors:**
```
Failed to start container: port already in use
→ Solution: Stop conflicting services or restart Docker
```

## Development Focus Areas

### Framework Packages (src/Lewee.*)

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

**Shared Utilities (Lewee.Shared)**
- Cross-cutting concerns
- Logging constants
- HTTP headers
- Extension methods
- **Key Pattern:** Zero-dependency utilities

### Sample Application (sample/Pizzeria.*)

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

### Contribution Guidelines

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

## Decision-Making Guide

### Choosing the Right Approach

**Question: Should I add a new NuGet package?**
```
Is the functionality critical?
├─ YES → Does it already exist in current packages?
│   ├─ YES → Use existing package
│   └─ NO → Is it a stable, well-maintained package?
│       ├─ YES → Add to Directory.Packages.props
│       └─ NO → Implement functionality directly
└─ NO → Implement using existing dependencies
```

**Question: Where should I put this code?**
```
What does the code do?
├─ Business logic → Lewee.Domain or [Project].Domain
├─ Use case orchestration → Lewee.Application or [Project].Application
├─ Database/API concerns → Lewee.Infrastructure.* or [Project].Data/Api
├─ Cross-cutting utilities → Lewee.Shared
└─ Presentation/UI → [Project].Api or Lewee.Blazor
```

**Question: What type of test should I write?**
```
What are you testing?
├─ Business rules → Unit test in Domain.Tests
├─ Application logic → Unit test in Application.Tests
├─ Database queries → Integration test
├─ API endpoints → Integration test
└─ End-to-end scenarios → Integration test with Aspire
```

### Architectural Constraints

**Must Follow:**
- Domain layer has no dependencies on other layers
- Application layer depends only on Domain
- Infrastructure implements interfaces from Domain/Application
- Use dependency injection for all cross-layer dependencies
- Maintain clean architecture boundaries

**Must Not:**
- Reference infrastructure from domain layer
- Add business logic to controllers/endpoints
- Use concrete classes where interfaces exist
- Skip validation for commands
- Ignore existing patterns

### Performance Considerations

**Optimize for:**
- Fast build times (12-20 seconds target)
- Quick unit tests (< 30 seconds total)
- Efficient database queries (use EF properly)

**Don't Optimize Prematurely:**
- Integration test speed (containers need time)
- First-time package restore (unavoidable)

## Technology Stack Reference

### Core Dependencies

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

### Package Version Strategy

**Pinned Versions:**
- MediatR 12.5.0 (last free version)
- FluentValidation 8.7.0 (last free version)

**Latest Versions:**
- All Microsoft packages (.NET, EF Core, Aspire)
- Supporting libraries (Npgsql, FastEndpoints, etc.)

**Rationale:** Balance between stability and staying current with .NET ecosystem