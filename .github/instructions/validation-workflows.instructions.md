---
applyTo: "**/*.cs,**/*.csproj"
---

# Validation Workflows

## Required Validation After Changes

**Decision Tree:**
```
Did you change framework code (Lewee.*)?
├─ YES → Run all 4 validation workflows below
└─ NO → Did you change sample app (Pizzeria.*)?
    ├─ YES → Run workflows 1, 2, and 3
    └─ NO → Did you only change documentation?
        ├─ YES → Run workflow 1 only
        └─ NO → Run workflow 1 to be safe
```

## Workflow 1: Framework Build Validation

**When:** After any framework (Lewee.*) changes

**Commands:**
```bash
dotnet build --configuration Release --nologo
```

**Success Criteria:**
- All Lewee.* projects compile successfully
- Zero compilation warnings
- Zero style violations

## Workflow 2: Unit Test Validation

**When:** After code changes (not documentation-only)

**Commands:**
```bash
dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo
```

**Success Criteria:**
- All unit tests pass
- No test failures or exceptions
- Test execution time < 30 seconds

## Workflow 3: Integration Test Validation

**When:** After infrastructure or data layer changes

**Commands:**
```bash
dotnet test --filter "FullyQualifiedName~Integration" --configuration Release --no-build --nologo
```

**Prerequisites:**
- Docker Desktop running
- .NET Aspire workload installed

**Success Criteria:**
- All integration tests pass
- Database operations work correctly
- API endpoints respond as expected

**Note:** Aspire manages PostgreSQL test containers automatically

## Workflow 4: Package Validation

**When:** Before releasing framework updates

**Commands:**
```bash
dotnet pack --configuration Release --nologo --no-build
```

**Success Criteria:**
- NuGet packages created without errors
- Package versions are correct
- All dependencies properly referenced
