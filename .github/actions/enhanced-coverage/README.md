# Enhanced Code Coverage Action

This custom GitHub Action generates clean, consolidated code coverage reports using ReportGenerator, eliminating the duplicate entries issue found with other coverage reporting actions.

## Problem Solved

The `irongut/CodeCoverageSummary` action tends to create duplicate entries when multiple test projects generate coverage files, resulting in confusing reports like:

```
| Lewee.Domain | 49% | 50% | 86 | ❌
| Lewee.Shared | 83% | 38% | 8 | ✔
| Lewee.Shared | 83% | 62% | 8 | ✔  <- Duplicate
| Lewee.Domain | 20% | 0% | 86 | ❌  <- Duplicate
```

This action uses ReportGenerator to intelligently merge coverage data from multiple sources, showing each package only once with properly aggregated metrics.

## Usage

```yaml
- name: Generate Enhanced Code Coverage Report
  uses: ./.github/actions/enhanced-coverage
  with:
    coverage-reports: coverage/**/coverage.cobertura.xml
    output-file: code-coverage-results.md
    fail-under: 80
```

## Inputs

| Input | Description | Required | Default |
|-------|-------------|----------|---------|
| `coverage-reports` | Path pattern to coverage reports (e.g., `coverage/**/coverage.cobertura.xml`) | Yes | - |
| `output-file` | Output file path for the markdown report | No | `code-coverage-results.md` |
| `fail-under` | Minimum coverage percentage to pass | No | `0` |

## Outputs

| Output | Description |
|--------|-------------|
| `coverage-percentage` | Overall line coverage percentage |

## Example Output

The action generates a clean markdown table:

```markdown
## 📊 Code Coverage Report

|**Name**|**Covered**|**Uncovered**|**Coverable**|**Total**|**Line coverage**|**Covered**|**Total**|**Branch coverage**|
|:---|---:|---:|---:|---:|---:|---:|---:|---:|
|**Lewee.Domain**|**97**|**71**|**168**|**633**|**57.7%**|**18**|**36**|**50%**|
|**Lewee.Infrastructure.Data**|**0**|**272**|**272**|**876**|**0%**|**0**|**86**|**0%**|
|**Lewee.Shared**|**5**|**1**|**6**|**38**|**83.3%**|**5**|**8**|**62.5%**|

**Overall Coverage:** 21.8%
- **Line Coverage:** 21.8% (102/467 lines)
- **Branch Coverage:** 17.1%
```

## Dependencies

- .NET SDK (for ReportGenerator tool)
- Coverage files in Cobertura XML format

## How It Works

1. Installs ReportGenerator as a global .NET tool
2. Processes all matching coverage files using ReportGenerator's intelligent merging
3. Extracts clean metrics from the generated markdown summary
4. Formats results for PR comments
5. Optionally enforces coverage thresholds