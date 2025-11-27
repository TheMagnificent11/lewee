---
applyTo: "**/*.cs,**/*.csproj,**/*.sln,**/*.slnx"
---

# Build and Test Commands

## Command Reference

| Task | Command | Typical Duration | Timeout Setting | Notes |
|------|---------|------------------|-----------------|-------|
| Clean | `dotnet clean lewee.sln` | ~2s | 60s | Safe to run anytime |
| Restore | `dotnet restore lewee.sln --nologo` | 2-30s | 120s | Depends on cache state |
| Build | `dotnet build lewee.sln --configuration Release --no-restore --nologo` | 12-20s | 120s | **NEVER CANCEL** |
| Full Rebuild | `dotnet build lewee.sln --configuration Release --no-incremental --nologo` | ~12s | 120s | **NEVER CANCEL** |
| Unit Tests | `dotnet test lewee.sln --configuration Release --no-build --nologo` | ~4s | 60s | Fast validation |
| Integration Tests | `dotnet test lewee.sln --configuration Release --no-build --nologo` | 300+s | 600s | Uses Aspire containers - **NEVER CANCEL** |
| Pack | `dotnet pack lewee.sln --configuration Release --nologo --no-build` | ~2s | 60s | Creates NuGet packages |

## Critical Rules

**NEVER:**
- Cancel builds or long-running commands before completion
- Use timeouts less than the recommended values
- Run integration tests without Docker Desktop running

**ALWAYS:**
- Wait for command completion
- Use `--nologo` flag to reduce output noise
- Run tests after making code changes
