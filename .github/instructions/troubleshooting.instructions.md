---
applyTo: "**/*"
---

# Troubleshooting Guide

## Problem Resolution Matrix

| Symptom | Likely Cause | Solution |
|---------|-------------|----------|
| Build fails with "NETSDK1045" | Wrong .NET version | Install .NET 10.0 SDK (see Environment Setup) |
| Integration tests fail to start | Aspire workload missing | `dotnet workload install aspire` |
| Aspire services won't start | Port conflicts | Check port availability, restart Docker |
| Unexplained build errors | Stale build artifacts | `dotnet clean lewee.sln` |
| Slow package restore | First run after clone | Normal - packages downloading from NuGet |
| Container startup failures | Docker not running | Start Docker Desktop |
| Test timeouts | Containers still starting | Wait longer, increase timeout |

## Debug Checklist

**Before asking for help:**
1. [ ] Verified .NET 10.0 SDK is installed (`dotnet --version`)
2. [ ] Ran `dotnet clean lewee.sln`
3. [ ] Checked Docker Desktop is running (for integration tests)
4. [ ] Reviewed error message carefully
5. [ ] Tried the solution from the matrix above

## Common Error Patterns

**Build Errors:**
```
NETSDK1045: The current .NET SDK does not support targeting .NET 10.0
→ Solution: Install .NET 10.0 SDK
```

**Test Errors:**
```
Failed to start container: port already in use
→ Solution: Stop conflicting services or restart Docker
```
