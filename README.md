# Lewee

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.Net.

## Status

[![CI Build](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml/badge.svg)](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml)

## Dependencies

Below is summary of the dependencies used by Lewee. Note that this isn't a list of NuGet packages, just a high-level list of software used and each can have several related NuGet packages.

- [.Net 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Serilog](https://serilog.net)
- [Entity Framework](https://learn.microsoft.com/en-us/ef) (using SQL Server)
- [Mediatr](https://github.com/jbogard/MediatR)
- [FastEndpoints](https://fast-endpoints.com)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest)
- [Ardalis.Specification](http://specification.ardalis.com)
- [xUnit](https://xunit.net)

## Running the sample application

### Prerequisites

- .Net CLI
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### CLI

Execute the following in a terminal at the root of this Git repository.

```bash
docker compose up -d
dotnet run --project .\sample\Sample.Restaurant.Server\
```

Navigate to [https://localhost:54577](https://localhost:54577).

## Contributing

Lewee uses the [Gitflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) branching model, which keeps the `main` branch clean and essentially represents the latest version available in production.

The `develop` branch is the default branch and what you should branch from to implement features and fix bugs.

Both `main` and `develop` have branch protection rules applied and can only take commits via pull requests.

The `develop` branch protection rules require a the PR branch to be up-to-date with `develop` and have a [linear history](https://www.bitsnbites.eu/a-tidy-linear-git-history/).
