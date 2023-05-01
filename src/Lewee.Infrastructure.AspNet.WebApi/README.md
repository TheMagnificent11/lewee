# Lewee.Infrastructure.AspNet.WebApi

This package assists with configuring and developing for ASP.Net Web API projects.

## Dependencies

- [Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md)

## Features

- CORS configuration
- Controller configuration
- [ApiControllerBase](./ApiControllerBase.cs)
- Extension methods to convert [Result](../Lewee.Application/Mediation/Requests/Result.cs) to `IActionResult` with appropriate HTTP status codes

(`services` in the code examples below are `Microsoft.Extensions.DependencyInjection.ServicesCollection`.

### CORS Configuration

```cs
services.ConfigureCorsDefaultPolicy("https://allowed-orgin1.com;https://allowed-origin2.com");
```

### Controller Configuration

```cs
services.ConfigureControllers();
```

This configures the following:

- Controller names to use kebab-case when using the [Controller] attribute for routing naming
- Camel-casing for JSON serialization.
- [ProblemDetails](TODO)
