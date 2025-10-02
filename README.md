# Lewee

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.Net.

## Status

[![CI Build](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml/badge.svg)](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml)

## Dependencies

Below is summary of the dependencies used by Lewee. Note that this isn't a list of NuGet packages, just a high-level list of software used and each can have several related NuGet packages.

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [PostgreSQL](https://www.postgresql.org/) with [Npgsql](https://www.npgsql.org/)
- [Entity Framework](https://learn.microsoft.com/en-us/ef)
- [FreeMediator](https://github.com/khellang/FreeMediator)
- [FastEndpoints](https://fast-endpoints.com)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest)
- [Ardalis.Specification](http://specification.ardalis.com)
- [xUnit](https://xunit.net)

## Running the sample application

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=linux#install-net-aspire) (`dotnet workload install aspire`)

### CLI

Execute the following in a terminal at the root of this Git repository.

```bash
dotnet run --project ./sample/Pizzeria.AppHost/
```

This will start the .NET Aspire dashboard and orchestrate the PostgreSQL database and Pizzeria Store API.

Navigate to the Aspire dashboard (typically [https://localhost:17268](https://localhost:17268)) to monitor services and view logs.

The Pizzeria Store API will be available at the URL shown in the Aspire dashboard.
