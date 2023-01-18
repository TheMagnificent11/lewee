# Lewee

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.Net.

## Dependencies

- [.Net 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Serilog](https://serilog.net)
- [Seq](https://datalust.co/seq)
- [Entity Framework 7](https://learn.microsoft.com/en-us/ef) (using SQL Server)
- [Mediatr](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest)
- [Mapster](https://github.com/MapsterMapper/Mapster)

## Running the sample application

### Prerequisites

- .Net CLI
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Contributing

Lewee usings the [Gitflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) branching model, which keeps the `master`/`main` branch very clean and essentially represents the latest version available in production.

The `develop` branch is the default branch and what you should branch from to implement features and fix bugs.

Both `master` and `develop` have branch protection rules applied and can only take commits via pull requests.

The `develop` branch protection rules require a the PR branch to be up-to-date with `develop` and have a [linear history](https://www.bitsnbites.eu/a-tidy-linear-git-history/).
