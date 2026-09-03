# Lewee Development Instructions

This file provides an overview and links to detailed Copilot instructions for different aspects of development in this repository.

**Important:** Please keep this file and the instruction files in the `.github/instructions/` directory up-to-date as the codebase evolves. When making changes to the repository structure, build process, coding standards, or other documented aspects, update the relevant instruction files to ensure they remain accurate and helpful.

## Instruction Files

The following instruction files are available in the `.github/instructions/` directory. Each file contains detailed guidance for specific areas:

| Instruction File | Description | Applies To |
| ------------------ | ------------- | ------------ |
| [overview.instructions.md](instructions/overview.instructions.md) | Project overview and introduction | All files |
| [how-to-use.instructions.md](instructions/how-to-use.instructions.md) | How to use these instructions effectively | All files |
| [visual-studio-solution.instructions.md](instructions/visual-studio-solution.instructions.md) | Visual Studio solution configuration | Solution and project files |
| [environment-setup.instructions.md](instructions/environment-setup.instructions.md) | Environment prerequisites and setup | All files |
| [build-and-test.instructions.md](instructions/build-and-test.instructions.md) | Build and test commands reference | C# source files |
| [sample-application.instructions.md](instructions/sample-application.instructions.md) | Sample Pizzeria application guidance | Sample directory |
| [code-quality.instructions.md](instructions/code-quality.instructions.md) | Code quality standards and enforcement | C# source files |
| [validation-workflows.instructions.md](instructions/validation-workflows.instructions.md) | Validation workflows for changes | C# source files |
| [repository-structure.instructions.md](instructions/repository-structure.instructions.md) | Repository structure and organization | All files |
| [troubleshooting.instructions.md](instructions/troubleshooting.instructions.md) | Troubleshooting common issues | All files |
| [development-focus.instructions.md](instructions/development-focus.instructions.md) | Development focus areas and contribution guidelines | C# source files |
| [decision-making.instructions.md](instructions/decision-making.instructions.md) | Decision-making guide for development choices | All files |
| [technology-stack.instructions.md](instructions/technology-stack.instructions.md) | Technology stack reference | All files |
| [blazor.instructions.md](instructions/blazor.instructions.md) | Blazor component development guidelines | Razor and Razor code-behind files |

## Quick Reference

### Priority Order

1. First, consult these instructions for repository-specific guidance
2. Then, use search or bash commands only when you encounter unexpected information

### Success Criteria

Your changes should:

- Build without errors or warnings
- Pass all existing tests
- Follow the established coding patterns
- Maintain backward compatibility for framework packages
- Be minimal and focused

### Key Commands

```bash
# Build
dotnet build --configuration Release --nologo

# Test (unit tests only)
dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo

# Format
dotnet format
```

## Directory-Specific AGENTS Files

Each major directory contains an AGENTS.md file that references the relevant Copilot instructions:

- `src/AGENTS.md` - Framework packages development
- `tests/AGENTS.md` - Framework tests guidance
- `sample/AGENTS.md` - Sample application development
- `sample/Pizzeria.Store.Web/AGENTS.md` - Blazor component development
- `sample-tests/AGENTS.md` - Sample tests guidance
