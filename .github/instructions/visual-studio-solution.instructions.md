---
applyTo: "**/*.sln,**/*.slnx,**/*.csproj"
---

# Visual Studio Solution

**Do not use solution folders.**

There are directory folders for the `src`, `tests`, `sample` and `sample-tests` projects.

However, the C# projects are named so that test projects appear next to their corresponding source projects in the Visual Studio Solution Explorer.

**Solution File:** The repository uses `lewee.slnx`.

## Solution File Standards

### Ordering

All entries in `lewee.slnx` must be ordered alphanumerically:

- `<File>` elements within each `<Folder>` section
- `<Project>` elements in the project list
- Maintain alphabetical order by path for both files and projects

This ensures consistency and makes the solution file easier to review and maintain.

### Id Attribute

**Do not use the `Id` attribute** on `<Project>` elements in the solution file.

The `Id` attribute is optional and adds unnecessary complexity. Visual Studio will function correctly without it.
