# Lewee

Lewee is an opinionated set of packages to assist with setting up a domain-driven design architecture within ASP.Net.

## Status
[![CI Build](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml/badge.svg)](https://github.com/TheMagnificent11/Lewee/actions/workflows/ci.yml)

## Dependencies

Below is summary of the dependencies used by Lewee. Note that this isn't a list of NuGet packages, just a high-level list of software used and each can have several related NuGet packages.

- [.Net 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Serilog](https://serilog.net)
- [Entity Framework 7](https://learn.microsoft.com/en-us/ef) (using SQL Server)
- [Mediatr](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest)
- [Mapster](https://github.com/MapsterMapper/Mapster)

## Running the sample application

### Prerequisites

- .Net CLI
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Visual Studio

1. Open `Lewee.sln` in Visual Studio
2. Set the startup project to `docker-compose`
3. Build the solution
4. Run/Debug the solution

Visual Studio/Docker Compose picks a localhost port to host the sample application and will launch the appropriate browser tabes.

### CLI

Execute the following in a terminal at the root of this Git repository.

```bash
docker compose up
```

Check the "Containers" section of Docker Desktop to determine the localhost port being used to host the sample application.

### VS Code

You can use the [CLI](#cli) steps to run the sample application.

Don't use the `Docker: Compose Up` command from the Command Palette because it does not include the overrides.

Unfortunately, there is now easy way to debug a Docker Compose application using VS Code.

[This Gist](https://gist.github.com/kendrahavens/cd3ba570e2e59fb4b0cbfe7e3529f0b5) that explains how it can be done, but it hasn't been tested for this solution/workspace.

## Contributing

Lewee usings the [Gitflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) branching model, which keeps the `master`/`main` branch very clean and essentially represents the latest version available in production.

The `develop` branch is the default branch and what you should branch from to implement features and fix bugs.

Both `master` and `develop` have branch protection rules applied and can only take commits via pull requests.

The `develop` branch protection rules require a the PR branch to be up-to-date with `develop` and have a [linear history](https://www.bitsnbites.eu/a-tidy-linear-git-history/).
