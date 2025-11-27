# Pizzeria Sample Application Tests

This directory contains tests for the Pizzeria sample application.

## Copilot Instructions

For development guidance in this directory, see the following instruction files:

- [Code Quality Instructions](../.github/instructions/code-quality.instructions.md) - Testing standards
- [Validation Workflows](../.github/instructions/validation-workflows.instructions.md) - Test validation workflows

## Test Projects

| Project | Description |
|---------|-------------|
| `Pizzeria.Store.Domain.Tests` | Unit tests for domain models |
| `Pizzeria.Store.Application.Tests.Unit` | Unit tests for application layer |
| `Pizzeria.Store.Web.Tests.Unit` | Unit tests for Blazor components |
| `Pizzeria.Tests.Integration` | End-to-end integration tests |

## Running Tests

```bash
# Run sample unit tests
dotnet test sample-tests/ --filter "FullyQualifiedName!~Integration" --configuration Release --nologo

# Run integration tests (requires Docker)
dotnet test sample-tests/Pizzeria.Tests.Integration/ --configuration Release --nologo
```

## Integration Tests

Integration tests require:
- Docker Desktop running
- .NET Aspire workload installed (`dotnet workload install aspire`)

The integration tests use Aspire to manage test containers automatically.

## Testing Standards

- Test method names should use underscores to describe behavior (e.g., `MethodName_Condition_ExpectedResult`)
- The `.editorconfig` in this directory suppresses CA1707 to allow underscores in test method names
- For Blazor component tests, do not use magic strings for selectors - expose constants from components
