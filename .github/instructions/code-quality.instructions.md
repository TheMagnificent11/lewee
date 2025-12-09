---
applyTo: "**/*.cs,**/*.csproj"
---

# Code Quality Standards

## Enforcement Rules

| Rule | Status | Impact |
|------|--------|--------|
| Warnings as Errors | Enabled | Build fails on any warning |
| Code Style in Build | Enforced | Style violations break the build |
| Documentation Generation | Required | All framework projects must generate XML docs |
| Code Coverage | Required for Framework | Pull requests with changes to `src/` directory (Lewee packages) must have at least 90% line coverage |

## Dependency Management

The solution uses Central Package Management via `Directory.Packages.props`.

Do not unnecessarily add package and project references; use implicit references where possible.

Therefore, always check for existing references in packages and projects that are already referenced implicitly in a C# project before adding new ones.

Furthermore, when working on application C# projects like web applications, do not add a reference if it comes in the `Microsoft.NET.Sdk.Web` web SDK.

**Exception:** During major framework upgrades (e.g., .NET 9 to .NET 10), explicit package references may be temporarily required to resolve version conflicts with third-party packages that have hard upper-bound constraints. These should be documented and removed once the third-party packages are updated for the new framework version.

## Project File Configuration

**IMPORTANT: Do NOT add build properties to individual `.csproj` files.**

All build configuration is centrally managed through a hierarchy of configuration files to ensure consistency across the solution.

### Configuration File Hierarchy

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

## Coding Style

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
- [ ] Limit lines to a maximum of 120 characters where possible, using one parameter or statement per line

## Logging

- Use logging scopes where possible to provide context as opposed to structured properties within a log message
  - Prefer to inherit structured properties from the scope when they are passed in as parameters
    - Values like CorrelationId, TenantId, UserId etc that are passed in as method parameters should be added to the logging scope at the entry point of the request
- Do not use emojis in log messages

## Dependency Injection

- Interface are public but implementations are internal by default
  - Register implementations in DI within the same assembly and expose an `IServiceCollection` extension method for registration
  - There may be a user case for public implementations, discuss with repository owner if needed

## ASP.NET HTTP Application Standards

- `Program.cs` should be minimal, only containing service registrations and middleware registrations
  - Do not add any logic to `Program.cs`
- Use extension methods on `IServiceCollection` and `IApplicationBuilder` to organize service and middleware registrations respectively

## Playwright/bUnit Testing Standards

- Do not use magic strings for selectors
  - Expose a constant from the component and use that instead e.g. `MainLayout.Selectors.SignOutButton`
  - Selector should use structural and semantic attributes where possible, avoid relying on CSS classes or element hierarchy
    - For example `[role='heading'][aria-level='1']`
