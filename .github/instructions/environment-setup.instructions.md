---
applyTo: "**/*"
---

# Environment Setup

## Prerequisites (Required)

| Component | Version | Installation Command | Verification |
| ----------- | --------- | --------------------- | -------------- |
| .NET SDK | 10.0+ | `curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh \| bash /dev/stdin --channel 10.0 --install-dir ~/.dotnet` | `dotnet --version` |
| Aspire CLI | 13.6.4 | `dotnet tool install --global Aspire.Cli --version 13.6.4` | `aspire --version` |
| Docker Desktop | Latest | Platform-specific | `docker --version` |

The Pizzeria AppHost uses `AspireUseCliBundle`, so install the Aspire CLI version matching its `Aspire.AppHost.Sdk` version. The current version is 13.6.4.

## Running the Full Test Suite

The root-level `dotnet test` includes the Aspire-based `Pizzeria.Tests.E2E` tests. Before running it, set up Aspire, Docker, the HTTPS development certificates, and Playwright:

```bash
dotnet tool install --global Aspire.Cli --version 13.6.4
aspire certs trust
dotnet dev-certs https --trust
dotnet build --configuration Release --nologo
pwsh sample-tests/Pizzeria.Tests.E2E/bin/Release/net10.0/playwright.ps1 install --with-deps chromium
dotnet test --configuration Release --no-build --nologo
```

Docker must be running so Aspire can start the PostgreSQL and Keycloak containers. This setup follows `.github/workflows/build-and-test.yml`.

**PATH Configuration:**

```bash
export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
```

**Critical:** The repository targets .NET 10.0 and will not build with older versions.
