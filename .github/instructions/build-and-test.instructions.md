---
applyTo: "**/*.cs,**/*.csproj,**/*.sln,**/*.slnx"
---

# Build and Test Commands

## Command Reference

| Task | Command | Typical Duration | Timeout Setting | Notes |
| ------ | --------- | ------------------ | ----------------- | ------- |
| Clean | `dotnet clean lewee.slnx` | ~2s | 60s | Safe to run anytime |
| Restore | `dotnet restore lewee.slnx --nologo` | 2-30s | 120s | Depends on cache state |
| Build | `dotnet build lewee.slnx --configuration Release --no-restore --nologo` | 12-20s | 120s | **NEVER CANCEL** |
| Full Rebuild | `dotnet build lewee.slnx --configuration Release --no-incremental --nologo` | ~12s | 120s | **NEVER CANCEL** |
| Unit Tests | `dotnet test lewee.slnx --filter "FullyQualifiedName!~E2E" --configuration Release --no-build --nologo` | ~4s | 60s | Fast validation |
| E2E Tests | `dotnet test --configuration Release --no-build --nologo` | 300+s | 600s | Includes Aspire containers and Playwright; follow Environment Setup first - **NEVER CANCEL** |
| Pack | `dotnet pack lewee.slnx --configuration Release --nologo --no-build` | ~2s | 60s | Creates NuGet packages |

## Critical Rules

**NEVER:**

- Cancel builds or long-running commands before completion
- Use timeouts less than the recommended values
- Run E2E tests without Docker running or before installing Playwright browsers (see Environment Setup)

**ALWAYS:**

- Wait for command completion
- Use `--nologo` flag to reduce output noise
- Run tests after making code changes
