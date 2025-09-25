# Lewee Development Instructions

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.NET. This repository contains both the Lewee framework packages and a sample restaurant management application demonstrating their usage.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Environment Setup
- **CRITICAL**: Install .NET 9.0 SDK - the repository targets .NET 9.0 and will not build with older versions
- Install .NET 9.0: `curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0 --install-dir ~/.dotnet`
- Update PATH: `export PATH="~/.dotnet:$PATH"`
- Verify installation: `dotnet --version` (should show 9.0.x)
- Install .NET Aspire workload: `dotnet workload install aspire`

### Build and Test Process
- **NEVER CANCEL builds or long-running commands** - Wait for completion
- Clean solution: `dotnet clean lewee.sln` (takes ~2 seconds)
- Restore packages: `dotnet restore lewee.sln --nologo` (takes ~2-30 seconds depending on cache)
- Build solution: `dotnet build lewee.sln --configuration Release --no-restore --nologo` 
  - **TIMING**: Takes ~12-20 seconds. NEVER CANCEL. Set timeout to 120+ seconds minimum.
- Full rebuild: `dotnet build lewee.sln --configuration Release --no-incremental --nologo`
  - **TIMING**: Takes ~12 seconds. Set timeout to 120+ seconds minimum.
- Run unit tests: `dotnet test lewee.sln --configuration Release --no-build --nologo`
  - **TIMING**: Takes ~4 seconds. Set timeout to 60+ seconds.
- Run integration tests: `dotnet test lewee.sln --configuration Release --no-build --nologo`
  - **TIMING**: Takes 5+ minutes with containers. NEVER CANCEL. Set timeout to 600+ seconds minimum.
  - **NOTE**: Integration tests use TestContainers and .NET Aspire for PostgreSQL containers
- Pack NuGet packages: `dotnet pack lewee.sln --configuration Release --nologo --no-build`
  - **TIMING**: Takes ~2 seconds. Set timeout to 60+ seconds.

### Sample Application
The sample pizzeria application demonstrates Lewee framework usage:

Eventually other parts of the pizzeria will be added as services e.g. the pizzeria kitchen and pizzeria delivery.

The idea is use RabbitMQ to publish messages to these other services to carry out operations to fullfil the `Order` created by by the Pizzeria Store.

- **Prerequisites**: 
  - Docker Desktop for running containers
  - .NET Aspire workload installed: `dotnet workload install aspire`
  - Run the application: `dotnet run --project ./sample/Pizzeria.AppHost/`
  - Access Aspire dashboard (typically https://localhost:17268) to monitor services
  - PostgreSQL database is automatically managed by Aspire
- **Architecture**: Uses .NET Aspire for service orchestration and PostgreSQL for data storage  
- **Alternative**: Integration tests provide the best way to validate framework functionality
- **CONTAINER NOTE**: Aspire manages containers automatically - no manual Docker setup required

### Coding Style

The coding styles are defined in the `.editorconfig` file and `dotnet format` should be run to ensure code formatting compliance.

## Validation Scenarios

### Always Validate These Workflows After Making Changes:
1. **Framework Build Validation**:
   - Run complete build: `dotnet build lewee.sln --configuration Release --nologo`
   - Verify all Lewee.* projects compile successfully
   - Check for no compilation warnings in framework code

2. **Unit Test Validation**:
   - Run unit tests: `dotnet test`
   - All unit tests must pass
   - No test failures or exceptions

3. **Integration Test Validation** (uses .NET Aspire for containers):
   - Run integration tests: `dotnet test`
   - Tests validate database operations, API endpoints, and domain logic using PostgreSQL
   - **NOTE**: Aspire manages test containers automatically

4. **Package Validation**:
   - Run pack command: `dotnet pack lewee.sln --configuration Release --nologo`
   - Verify NuGet packages are created without errors

## Common Tasks

### Repository Structure
```
src/                    # Lewee framework packages
├── Lewee.Domain/       # Core domain abstractions
├── Lewee.Application/  # Application layer utilities
├── Lewee.Shared/       # Shared utilities
├── Lewee.Contracts/    # Contract definitions
├── Lewee.Infrastructure.Data/         # Entity Framework integration
├── Lewee.Infrastructure.PostgreSQL/   # PostgreSQL-specific integrations
├── Lewee.Infrastructure.AspNet/       # ASP.NET Core integration
├── Lewee.Infrastructure.AspNet.WebApi/ # Web API utilities
├── Lewee.Blazor/       # Blazor component library
└── Lewee.IntegrationTests/  # Integration testing helpers

sample/                 # Sample pizzeria application
├── Pizzeria.AppHost/              # .NET Aspire orchestration host
├── Pizzeria.ServiceDefaults/      # Shared Aspire service configurations
├── Pizzeria.Common/               # Common utilities and constants
├── Pizzeria.Store.Domain/         # Domain models
├── Pizzeria.Store.Application/    # Application services
├── Pizzeria.Store.Data/           # Data layer with PostgreSQL
├── Pizzeria.Store.Contracts/      # API contracts
└── Pizzeria.Store.Api/            # ASP.NET Core Web API

tests/                  # Unit tests for framework
sample-tests/           # Tests for sample application
├── Pizzeria.Tests.Integration/    # Integration tests using Aspire
└── Pizzeria.Store.Domain.Tests/   # Domain unit tests
```

### Key Files and Configuration
- `lewee.sln` - Main solution file with all projects
- `Directory.Build.props` - Global MSBuild properties (targets .NET 9.0)
- `Directory.Packages.props` - Centralized package management with Aspire dependencies
- `sample/Pizzeria.AppHost/Program.cs` - Aspire orchestration configuration
- `.github/workflows/ci.yml` - CI/CD pipeline configuration

### Frequently Used Commands
- Clean build artifacts: `dotnet clean lewee.sln` (takes ~2 seconds)
- Full rebuild: `dotnet build lewee.sln --configuration Release --no-incremental` (takes ~12 seconds)
- Run specific test project: `dotnet test tests/Lewee.Domain.Tests.Unit/` (takes ~3 seconds)
- Run specific integration test: `dotnet test sample-tests/Pizzeria.Tests.Integration/`
- Run sample application: `dotnet run --project sample/Pizzeria.AppHost/`
- Check for outdated packages: `dotnet list package --outdated`
- Pack packages: `dotnet pack lewee.sln --configuration Release --nologo`

### Code Quality
- The repository enforces `TreatWarningsAsErrors=true`
- Code style is enforced via `EnforceCodeStyleInBuild=true`
- All framework projects generate documentation files
- Test projects are excluded from code coverage requirements

## Critical Timing Guidelines
- **Clean**: 2 seconds typical, set timeout to 60+ seconds
- **Restore**: 2-30 seconds typical (depends on cache), set timeout to 120+ seconds
- **Build**: 12-20 seconds typical, set timeout to 120+ seconds  
- **Full Rebuild**: 12 seconds typical, set timeout to 120+ seconds
- **Unit Tests**: 4 seconds typical, set timeout to 60+ seconds
- **Specific Test Project**: 3 seconds typical, set timeout to 60+ seconds
- **Integration Tests**: 300+ seconds typical, set timeout to 600+ seconds
- **Pack**: 2 seconds typical, set timeout to 60+ seconds
- **NEVER CANCEL**: Any build or test command - always wait for completion

## Troubleshooting
- If build fails with "NETSDK1045" error, install .NET 9.0 SDK
- If integration tests fail, ensure .NET Aspire workload is installed: `dotnet workload install aspire`
- If Aspire services fail to start, check that required ports are available
- Clean solution if experiencing unexplained build errors: `dotnet clean lewee.sln`
- If restore is slow, packages may be downloading - this is normal on first run

## Development Focus Areas
- **Domain Layer**: Core business logic and abstractions in `Lewee.Domain`
- **Application Layer**: CQRS, validation, and application services in `Lewee.Application`
- **Infrastructure**: Entity Framework, PostgreSQL, ASP.NET Core, and Blazor integrations
- **Sample App**: Demonstrates framework usage patterns and best practices using Aspire orchestration