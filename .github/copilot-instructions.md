# Lewee Development Instructions

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.NET. This repository contains both the Lewee framework packages and a sample restaurant management application demonstrating their usage.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Environment Setup
- **CRITICAL**: Install .NET 9.0 SDK - the repository targets .NET 9.0 and will not build with older versions
- Install .NET 9.0: `curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0 --install-dir ~/.dotnet`
- Update PATH: `export PATH="~/.dotnet:$PATH"`
- Verify installation: `dotnet --version` (should show 9.0.x)
- Docker Desktop for running SQL Server and Seq logging (optional for framework development, required for sample app)

### Build and Test Process
- **NEVER CANCEL builds or long-running commands** - Wait for completion
- Clean solution: `dotnet clean lewee.sln` (takes ~2 seconds)
- Restore packages: `dotnet restore lewee.sln --nologo` (takes ~2-30 seconds depending on cache)
- Build solution: `dotnet build lewee.sln --configuration Release --no-restore --nologo` 
  - **TIMING**: Takes ~12-20 seconds. NEVER CANCEL. Set timeout to 120+ seconds minimum.
- Full rebuild: `dotnet build lewee.sln --configuration Release --no-incremental --nologo`
  - **TIMING**: Takes ~12 seconds. Set timeout to 120+ seconds minimum.
- Run unit tests: `dotnet test lewee.sln --configuration Release --no-build --nologo --filter "Category!=Integration"`
  - **TIMING**: Takes ~4 seconds. Set timeout to 60+ seconds.
- Run integration tests: `dotnet test lewee.sln --configuration Release --no-build --nologo --filter "Category=Integration"`
  - **TIMING**: Takes 5+ minutes with Docker containers. NEVER CANCEL. Set timeout to 600+ seconds minimum.
  - **NOTE**: Integration tests use TestContainers and require Docker to be running
- Pack NuGet packages: `dotnet pack lewee.sln --configuration Release --nologo --no-build`
  - **TIMING**: Takes ~2 seconds. Set timeout to 60+ seconds.

### Sample Application
The sample restaurant management application demonstrates Lewee framework usage:

- **Prerequisites**: 
  - Docker Compose must be running: `docker compose up -d`
  - SQL Server will be available on localhost:5434
  - Seq logging will be available on localhost:5341
- **KNOWN ISSUE**: Sample application currently has Swagger configuration problems and may not start properly
- **Alternative**: Integration tests provide the best way to validate framework functionality
- **DOCKER NOTE**: If Docker is not available in your environment, focus on unit tests and framework development rather than sample application or integration tests

## Validation Scenarios

### Always Validate These Workflows After Making Changes:
1. **Framework Build Validation**:
   - Run complete build: `dotnet build lewee.sln --configuration Release --nologo`
   - Verify all Lewee.* projects compile successfully
   - Check for no compilation warnings in framework code

2. **Unit Test Validation**:
   - Run unit tests: `dotnet test --filter "Category!=Integration"`
   - All unit tests must pass
   - No test failures or exceptions

3. **Integration Test Validation** (if Docker is available):
   - Start Docker services: `docker compose up -d` 
   - Wait for containers to be ready (may take 1-2 minutes for SQL Server)
   - Run integration tests: `dotnet test --filter "Category=Integration"`
   - Tests validate database operations, API endpoints, and domain logic
   - **NOTE**: Skip this if Docker is not available in your environment

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
├── Lewee.Infrastructure.Data/      # Entity Framework integration
├── Lewee.Infrastructure.AspNet/    # ASP.NET Core integration
├── Lewee.Infrastructure.AspNet.WebApi/  # Web API utilities
├── Lewee.Blazor/       # Blazor component library
└── Lewee.IntegrationTests/  # Integration testing helpers

sample/                 # Sample restaurant application
├── Sample.Restaurant.Domain/        # Domain models
├── Sample.Restaurant.Application/   # Application services
├── Sample.Restaurant.Infrastructure/ # Data layer
├── Sample.Restaurant.Contracts/     # API contracts
├── Sample.Restaurant.Client/        # Blazor WebAssembly UI
└── Sample.Restaurant.Server/        # ASP.NET Core host

tests/                  # Unit tests for framework
sample-tests/           # Tests for sample application
```

### Key Files and Configuration
- `lewee.sln` - Main solution file with all projects
- `Directory.Build.props` - Global MSBuild properties (targets .NET 9.0)
- `Directory.Packages.props` - Centralized package management
- `docker-compose.yml` - SQL Server and Seq services
- `.github/workflows/ci.yml` - CI/CD pipeline configuration

### Frequently Used Commands
- Clean build artifacts: `dotnet clean lewee.sln` (takes ~2 seconds)
- Full rebuild: `dotnet build lewee.sln --configuration Release --no-incremental` (takes ~12 seconds)
- Run specific test project: `dotnet test tests/Lewee.Domain.Tests.Unit/` (takes ~3 seconds)
- Run specific integration test: `dotnet test sample-tests/Sample.Restaurant.Server.Tests.Integration/`
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
- If integration tests fail, ensure Docker is running and accessible
- If Docker is not available, focus on unit tests - integration tests require Docker containers
- If sample app fails to start, this is a known Swagger configuration issue
- Clean solution if experiencing unexplained build errors: `dotnet clean lewee.sln`
- If restore is slow, packages may be downloading - this is normal on first run

## Development Focus Areas
- **Domain Layer**: Core business logic and abstractions in `Lewee.Domain`
- **Application Layer**: CQRS, validation, and application services in `Lewee.Application`
- **Infrastructure**: Entity Framework, ASP.NET Core, and Blazor integrations
- **Sample App**: Demonstrates framework usage patterns and best practices