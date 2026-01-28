# Lewee

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.NET.

## Status

[![CI Build](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml/badge.svg)](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml)

## Framework Packages

| Package | Description |
|---------|-------------|
| `Lewee.Domain` | Domain layer abstractions including entities, aggregate roots, value objects, domain events, specifications, and repository interfaces |
| `Lewee.Application` | Application layer with CQRS using MediatR, pipeline behaviors for logging, validation, and exception handling |
| `Lewee.Common` | Cross-cutting utilities including result types, client messaging contracts, logging constants, and HTTP headers |
| `Lewee.Infrastructure.Data` | Entity Framework Core integration with repository implementation and domain event dispatching |
| `Lewee.Infrastructure.PostgreSQL` | PostgreSQL-specific database configuration using Npgsql |
| `Lewee.Infrastructure.AspNet` | ASP.NET Core integration including authenticated user services and correlation ID middleware |
| `Lewee.Infrastructure.Auth` | Authentication infrastructure with authenticated user service implementation |
| `Lewee.Infrastructure.Correlate` | Correlation ID infrastructure using the Correlate library |
| `Lewee.Infrastructure.FastEndpoints` | FastEndpoints-based command and query endpoint base classes |
| `Lewee.Infrastructure.Fluxor` | Fluxor state management infrastructure for Blazor applications with client event receiver |
| `Lewee.Infrastructure.Keycloak` | Keycloak OpenID Connect authentication integration with .NET Aspire service discovery |
| `Lewee.Infrastructure.Refit` | Refit HTTP client infrastructure with authentication and correlation ID support |
| `Lewee.Blazor` | Blazor client-side infrastructure with Fluxor and API integration |
| `Lewee.Playwright` | Playwright browser automation utilities for integration testing |

## Dependencies

Below is a summary of the dependencies used by Lewee. Note that this isn't a list of NuGet packages, just a high-level list of software used and each can have several related NuGet packages.

- [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [PostgreSQL](https://www.postgresql.org/) with [Npgsql](https://www.npgsql.org/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef)
- [MediatR](https://mediatr.io/) version 12.5.0 (latest free version)
- [FastEndpoints](https://fast-endpoints.com)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest) version 8.7.0 (free version)
- [Fluxor](https://github.com/mrpmorris/Fluxor) for Blazor state management
- [xUnit](https://xunit.net)

## Running the Sample Application

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=linux#install-net-aspire) (`dotnet workload install aspire`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL container)

### CLI

Execute the following in a terminal at the root of this Git repository.

```bash
dotnet run --project ./sample/Pizzeria.AppHost/
```

This will start the .NET Aspire dashboard and orchestrate the PostgreSQL database, Pizzeria Store Blazor Server application.

Navigate to the Aspire dashboard to monitor services and view logs.

The Pizzeria Store API Web application will be available at the URLs shown in the Aspire dashboard.
