# Lewee.Infrastructure.AspNet.WebApi

This package assists with configuring and developing for ASP.Net Web API projects.

## Dependencies

- [Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md)
- [FastEndpoints](https://fast-endpoints.com/)

## Features

- CORS configuration
- [QueryEndpoint](./QueryEndpoint.cs) base class for query endpoints
- [CommandEndpoint](./CommandEndpoint.cs) base class for command endpoints

`services` in the code examples below are `Microsoft.Extensions.DependencyInjection.ServicesCollection`.

### CORS Configuration

```cs
services.ConfigureCorsDefaultPolicy("https://allowed-orgin1.com;https://allowed-origin2.com");
```

