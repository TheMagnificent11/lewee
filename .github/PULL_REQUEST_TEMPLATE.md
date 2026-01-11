# <!-- Provide a PR title ->

## Summary

<!-- Provide a clear and concise description of the changes -->

## Type of Change

<!-- Mark all that apply with an "x" -->

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Refactoring (no functional changes)
- [ ] Performance improvement
- [ ] Code quality improvement

## Scope

<!-- Mark all that apply with an "x" -->

- [ ] Framework packages (src/Lewee.*)
- [ ] Sample application (sample/Pizzeria.*)
- [ ] Tests (tests/ or sample-tests/)
- [ ] Documentation
- [ ] Build/CI configuration

## Changes Made

<!-- List the specific changes made in this PR -->

-
-
-

## Related Issues

<!-- Link to related issues using #issue_number -->

Closes #
Relates to #

## Validation Checklist

<!-- Mark completed items with an "x" -->

### Build & Tests

- [ ] Code builds without errors: `dotnet build --configuration Release --nologo`
- [ ] Unit tests pass: `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo`
- [ ] Integration tests pass (if applicable): `dotnet test --filter "FullyQualifiedName~Integration" --configuration Release --no-build --nologo`

### Code Quality

- [ ] No compiler warnings
- [ ] Code formatted: `dotnet format`
- [ ] No StyleCop violations
- [ ] No SonarAnalyzer violations
- [ ] XML documentation added for public/protected APIs (framework packages only)
- [ ] Framework changes have at least 90% line coverage

### Documentation

- [ ] README files updated (if applicable)
- [ ] Code comments added where necessary
- [ ] Breaking changes documented
- [ ] Migration guide provided (if breaking changes)

### Solution & Project Files

- [ ] Solution file entries ordered alphanumerically
- [ ] No `Id` attributes in solution file
- [ ] Project references are correct
- [ ] Central Package Management followed (Directory.Packages.props)

### Framework Packages (if applicable)

- [ ] Backward compatibility maintained
- [ ] New packages added to root README.md
- [ ] New packages added to src/README.md
- [ ] Package versioning follows SemVer
- [ ] No unnecessary package or project references added

### Sample Application (if applicable)

- [ ] Sample demonstrates framework usage correctly
- [ ] No XML documentation comments in sample code
- [ ] Follows existing patterns and conventions

## Breaking Changes

<!-- Describe any breaking changes and migration path -->

None / N/A

<!-- OR -->

**Breaking Changes:**

-
-

**Migration Guide:**

-
-

## Additional Notes

<!-- Any additional information that reviewers should know -->

## Reviewer Checklist

<!-- For reviewers to complete -->

- [ ] Code follows repository coding standards
- [ ] Changes are minimal and focused
- [ ] Tests adequately cover the changes
- [ ] Documentation is clear and complete
- [ ] No unrelated changes included
- [ ] Commit messages are clear and descriptive
