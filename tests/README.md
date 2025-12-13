# Lewee Framework Tests

This directory contains unit tests for the Lewee framework packages.

## Copilot Instructions

For development guidance in this directory, see the following instruction files:

- [Code Quality Instructions](../.github/instructions/code-quality.instructions.md) - Testing standards
- [Validation Workflows](../.github/instructions/validation-workflows.instructions.md) - Test validation workflows

## Test Projects

| Project | Tests For |
|---------|-----------|
| `Lewee.Domain.Tests.Unit` | Domain layer abstractions and base classes |
| `Lewee.Application.Tests.Unit` | Application layer and CQRS implementation |
| `Lewee.Common.Tests.Unit` | Cross-cutting utilities and result types |
| `Lewee.Infrastructure.Data.Tests.Unit` | Entity Framework Core integration (unit tests) |
| `Lewee.Infrastructure.Data.Tests.Integration` | Entity Framework Core integration (integration tests) |
| `Lewee.Blazor.Tests.Integration` | Blazor component library tests |
| `Lewee.Tests.Common` | Shared test utilities |

## Running Tests

```bash
# Run all unit tests
dotnet test lewee.sln --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo

# Run specific test project
dotnet test tests/Lewee.Domain.Tests.Unit/ --configuration Release --nologo
```

## Coverage Requirements

Framework changes require at least 90% line coverage. Use the test coverage script to verify:

```bash
./test-coverage.ps1
```

## Testing Standards

- Test method names should use underscores to describe behavior (e.g., `MethodName_Condition_ExpectedResult`)
- The `.editorconfig` in this directory suppresses CA1707 to allow underscores in test method names
