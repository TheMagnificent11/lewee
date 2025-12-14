# Diff Coverage Check Action

A composite GitHub Action that checks code coverage for changed source files and enforces a minimum coverage threshold.

## Features

- Analyzes only files that have changed in a pull request
- Supports Cobertura XML coverage format
- Configurable coverage threshold
- Detailed reporting with actionable feedback
- Handles edge cases (non-coverable code, missing coverage data)

## Usage

```yaml
- name: Check Diff Coverage
  uses: ./.github/actions/diff-coverage-check
  with:
    coverage-file: coverage/coverage.cobertura.xml
    threshold: 90
    base-sha: ${{ github.event.pull_request.base.sha }}
    head-sha: ${{ github.sha }}
```

## Inputs

| Input | Description | Required | Default |
|-------|-------------|----------|---------|
| `coverage-file` | Path to the Cobertura coverage XML file | Yes | `coverage/coverage.cobertura.xml` |
| `threshold` | Minimum coverage percentage threshold (0-100) | Yes | `90` |
| `base-sha` | Base commit SHA to compare against | Yes | - |
| `head-sha` | Head commit SHA to compare | Yes | - |

## Behavior

1. **File Discovery**: Uses `git diff` to find changed `.cs` files in the `src/` directory
2. **Coverage Analysis**: Parses the Cobertura XML file to extract coverage data
3. **Threshold Enforcement**: Checks that each changed file meets the minimum coverage threshold
4. **Result**: Fails the workflow if any file is below the threshold

## Output Examples

### Success
```
🔍 Checking diff coverage for 2 changed files...
✅ src/Lewee.Domain/AggregateRoot.cs: 100.0% (16/16 lines)
✅ src/Lewee.Application/CommandResult.cs: 95.5% (21/22 lines)

📊 Coverage Summary:
   • Changed files: 2
   • Files with coverage data: 2
   • Files without coverage data: 0

✅ DIFF COVERAGE CHECK PASSED
   All 2 checked files meet the 90% coverage threshold!
```

### Failure
```
🔍 Checking diff coverage for 2 changed files...
❌ src/Lewee.Common/EnumExtensions.cs: 70.0% (14/20 lines)
✅ src/Lewee.Domain/AggregateRoot.cs: 100.0% (16/16 lines)

📊 Coverage Summary:
   • Changed files: 2
   • Files with coverage data: 2
   • Files without coverage data: 0

❌ DIFF COVERAGE CHECK FAILED
   1 of 2 checked files are below 90% coverage:
   - src/Lewee.Common/EnumExtensions.cs: 70.0% (need ~4 more covered lines)

💡 Please add tests to increase coverage for the files listed above.
```

## Requirements

- Python 3.x (available in GitHub Actions runners)
- Git (for diff analysis)
- Cobertura XML coverage report