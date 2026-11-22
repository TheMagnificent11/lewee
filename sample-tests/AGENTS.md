# Pizzeria Sample Application Tests

This directory contains tests for the Pizzeria sample application.

## Copilot Instructions

For development guidance in this directory, see the following instruction files:

- [Code Quality Instructions](../.github/instructions/code-quality.instructions.md) - Testing standards
- [Validation Workflows](../.github/instructions/validation-workflows.instructions.md) - Test validation workflows

## Test Projects

| Project | Description |
| --------- | ------------- |
| `Pizzeria.Store.Domain.Tests` | Unit tests for domain models |
| `Pizzeria.Ordering.StateManagement.Tests.Unit` | Unit tests for Fluxor state management |
| `Pizzeria.Ordering.Components.Tests.Unit` | Unit tests for shared Blazor components |
| `Pizzeria.Ordering.Web.Tests.Unit` | Unit tests for Blazor web application |
| `Pizzeria.Configuration.Tests.Unit` | Unit tests for sample configuration/seeding |
| `Pizzeria.Tests.E2E` | Aspire and Playwright end-to-end tests |

## Running Tests

```bash
# Run sample unit tests
dotnet test sample-tests/ --filter "FullyQualifiedName!~E2E" --configuration Release --nologo

# Run E2E tests (requires Docker and the prerequisites below)
dotnet test sample-tests/Pizzeria.Tests.E2E/ --configuration Release --nologo
```

## E2E Tests

E2E tests require:

- Docker running
- Aspire CLI 13.6.4 and trusted Aspire/HTTPS development certificates
- Playwright Chromium installed

See [Environment Setup](../.github/instructions/environment-setup.instructions.md) for the full root-level setup sequence. The tests use Aspire to manage test containers automatically.

## Testing Standards

- Test method names should use underscores to describe behavior (e.g., `MethodName_Condition_ExpectedResult`)
- The `.editorconfig` in this directory suppresses CA1707 to allow underscores in test method names
- For Blazor component tests, do not use magic strings for selectors - expose constants from components
