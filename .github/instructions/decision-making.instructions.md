---
applyTo: "**/*"
---

# Decision-Making Guide

## Choosing the Right Approach

**Question: Should I add a new NuGet package?**
```
Is the functionality critical?
├─ YES → Does it already exist in current packages?
│   ├─ YES → Use existing package
│   └─ NO → Is it a stable, well-maintained package?
│       ├─ YES → Add to Directory.Packages.props
│       └─ NO → Implement functionality directly
└─ NO → Implement using existing dependencies
```

**Question: Where should I put this code?**
```
What does the code do?
├─ Business logic → Lewee.Domain or [Project].Domain
├─ Use case orchestration → Lewee.Application or [Project].Application
├─ Database/API concerns → Lewee.Infrastructure.* or [Project].Data/Api
├─ Cross-cutting utilities → Lewee.Common
└─ Presentation/UI → [Project].Api or Lewee.Blazor
```

**Question: What type of test should I write?**
```
What are you testing?
├─ Business rules → Unit test in Domain.Tests
├─ Application logic → Unit test in Application.Tests
├─ Database queries → Integration test
├─ API endpoints → Integration test
└─ End-to-end scenarios → Integration test with Aspire
```

## Architectural Constraints

**Must Follow:**
- Domain layer has no dependencies on other layers
- Application layer depends only on Domain
- Infrastructure implements interfaces from Domain/Application
- Use dependency injection for all cross-layer dependencies
- Maintain clean architecture boundaries

**Must Not:**
- Reference infrastructure from domain layer
- Add business logic to controllers/endpoints
- Use concrete classes where interfaces exist
- Skip validation for commands
- Ignore existing patterns

## Performance Considerations

**Optimize for:**
- Fast build times (12-20 seconds target)
- Quick unit tests (< 30 seconds total)
- Efficient database queries (use EF properly)

**Don't Optimize Prematurely:**
- Integration test speed (containers need time)
- First-time package restore (unavoidable)
