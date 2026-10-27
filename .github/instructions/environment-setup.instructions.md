---
applyTo: "**/*"
---

# Environment Setup

## Prerequisites (Required)

| Component | Version | Installation Command | Verification |
| ----------- | --------- | --------------------- | -------------- |
| .NET SDK | 10.0+ | `curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh \| bash /dev/stdin --channel 10.0 --install-dir ~/.dotnet` | `dotnet --version` |
| .NET Aspire | Latest | `dotnet workload install aspire` | `dotnet workload list` |
| Aspire CLI | 13.6.4 | `dotnet tool install --global Aspire.Cli --version 13.6.4` | `aspire --version` |
| Docker Desktop | Latest | Platform-specific | `docker --version` |

Trust the Aspire development certificate with `aspire certs trust`.

**PATH Configuration:**

```bash
export PATH="~/.dotnet:$PATH"
```

**Critical:** The repository targets .NET 10.0 and will not build with older versions.
