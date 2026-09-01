# Lewee.Infrastructure.Refit

Domain-driven design infrastructure for [Refit](https://github.com/reactiveui/refit)-based HTTP client communication. This package provides infrastructure layer components that integrate Refit with domain-driven development patterns, including automatic authentication token propagation, distributed correlation ID tracking, and structured API exception handling that aligns with DDD principles.

## Dependencies

- [Refit.HttpClientFactory](https://github.com/reactiveui/refit) - Type-safe REST client with HttpClientFactory integration
- [Correlate.AspNetCore](https://github.com/skwasjer/Correlate) - Correlation ID functionality for ASP.NET Core
- [Lewee.Common](../Lewee.Common/README.md) - Shared utilities and constants
- [Lewee.Infrastructure.Auth](../Lewee.Infrastructure.Auth/README.md) - Authentication infrastructure including token handling

## Features

This infrastructure package provides:

- **Refit Client Configuration**: Streamlined setup of type-safe HTTP clients that integrate seamlessly with domain and application layers
- **Authentication Infrastructure**: Automatic Bearer token propagation from the current user context to external APIs
- **Correlation ID Infrastructure**: Built-in distributed tracing support using [Correlate](https://github.com/skwasjer/Correlate) to track requests across service boundaries

## Configuration

Infrastructure layer services are registered using standard ASP.NET Core dependency injection. In the code below, `services` is `Microsoft.Extensions.DependencyInjection.IServiceCollection`.

### Registering Refit API Clients

Use `AddWebApiHttpClient<T>` to configure type-safe Refit clients as infrastructure services:

```cs
using Lewee.Infrastructure.Refit;

// Register API client infrastructure with service discovery
services.AddWebApiHttpClient<IMyApiClient>("my-api-service");
```

This automatically configures infrastructure components:

- `AuthTokenDelegatingHandler` (from [Lewee.Infrastructure.Auth](../Lewee.Infrastructure.Auth/README.md)) - Infrastructure handler that adds Bearer tokens from the authentication context
- `CorrelatingHttpMessageHandler` (from Correlate) - Infrastructure handler that propagates correlation IDs across service boundaries

### Service Discovery Integration

The `AddWebApiHttpClient<T>` method integrates with .NET Aspire service discovery for infrastructure layer service location:

```cs
// In your AppHost (infrastructure orchestration)
var api = builder.AddProject<Projects.MyApi>("my-api-service");

// In your client application (infrastructure registration)
services.AddWebApiHttpClient<IMyApiClient>("my-api-service");
```

## Usage in Domain-Driven Design

### Defining Infrastructure Interfaces

Define Refit interfaces in your infrastructure layer to communicate with external APIs:

```cs
using Refit;

// Infrastructure interface for external API communication
public interface IOrdersApiClient
{
    [Get("/api/orders")]
    Task<IEnumerable<OrderDto>> GetOrdersAsync();

    [Get("/api/orders/{id}")]
    Task<OrderDto> GetOrderAsync(Guid id);

    [Post("/api/orders")]
    Task<OrderDto> CreateOrderAsync([Body] CreateOrderRequest request);

    [Put("/api/orders/{id}")]
    Task UpdateOrderAsync(Guid id, [Body] UpdateOrderRequest request);

    [Delete("/api/orders/{id}")]
    Task DeleteOrderAsync(Guid id);
}
```

Register the infrastructure client:

```cs
services.AddWebApiHttpClient<IOrdersApiClient>("orders-api");
```

### Using Infrastructure Clients in Application Services

Inject the infrastructure client into your application services or domain service implementations:

```cs
public class OrderService
{
    private readonly IOrdersApiClient ordersClient;

    public OrderService(IOrdersApiClient ordersClient)
    {
        this.ordersClient = ordersClient;
    }

    public async Task<OrderDto> GetOrderAsync(Guid id)
    {
        return await this.ordersClient.GetOrderAsync(id);
    }
}
```

### Integration with Application Layer Effects

When using with Fluxor state management in the application layer, handle infrastructure errors in effects:

```cs
using Fluxor;
using Lewee.Infrastructure.Refit;

public class OrderEffects
{
    private readonly IOrdersApiClient apiClient;
    private readonly ILogger<OrderEffects> logger;

    public OrderEffects(IOrdersApiClient apiClient, ILogger<OrderEffects> logger)
    {
        this.apiClient = apiClient;
        this.logger = logger;
    }

    [EffectMethod]
    public async Task CreateOrderAsync(CreateOrderAction action, IDispatcher dispatcher)
    {
        try
        {
            var order = await this.apiClient.CreateOrderAsync(action.Request);
            dispatcher.Dispatch(new CreateOrderSuccessAction
            {
                CorrelationId = action.CorrelationId,
                Order = order
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to create order");
            dispatcher.Dispatch(new CreateOrderFailureAction
            {
                CorrelationId = action.CorrelationId,
                ErrorMessage = ex.Message
            });
        }
    }
}
```

## Infrastructure Components

| Component | Description |
|-----------|-------------|
| **[ApiCientConfiguration.cs](./ApiCientConfiguration.cs)** | Infrastructure service registration with `AddWebApiHttpClient<T>` |

## Distributed Correlation ID Infrastructure

This infrastructure package uses [Correlate](https://github.com/skwasjer/Correlate) to automatically propagate correlation IDs across service boundaries. The `CorrelatingHttpMessageHandler` adds the `X-Correlation-ID` header to all outgoing HTTP requests.

Correlation IDs flow through the infrastructure layers:

1. Incoming HTTP requests (via Correlate middleware in infrastructure)
2. Outgoing HTTP requests (via `CorrelatingHttpMessageHandler` infrastructure)
3. Infrastructure and application layer logging
4. Response headers returned through infrastructure

### Infrastructure Configuration

The `AddWebApiHttpClient<T>` method automatically configures the Correlate infrastructure:

```cs
services.AddCorrelate(options => options.RequestHeaders = [RequestHeaders.CorrelationId]);
```

## Authentication Infrastructure

The `AuthTokenDelegatingHandler` (from [Lewee.Infrastructure.Auth](../Lewee.Infrastructure.Auth/README.md)) provides infrastructure-level authentication token handling:

1. Retrieves the `access_token` from the current authentication context (`HttpContext`)
2. Adds it as a `Bearer` token in the `Authorization` header for outgoing requests
3. Logs infrastructure warnings when authentication tokens are unavailable

This infrastructure is automatically configured when using `AddWebApiHttpClient<T>`.

### Infrastructure Prerequisites

For the authentication infrastructure to function:

1. `IHttpContextAccessor` must be registered in the DI container
2. The user must be authenticated with an `access_token` in the authentication context
3. `HttpContext` must be accessible (supported in Blazor Web Apps with Interactive Server and server-side rendering scenarios)

## Integration with Lewee DDD Layers

This infrastructure package integrates with other Lewee packages across the DDD layers:

| Package | Layer | Integration |
|---------|-------|-------------|
| **[Lewee.Common](../Lewee.Common/README.md)** | Cross-cutting | Provides `RequestHeaders.CorrelationId` constant for infrastructure |
| **[Lewee.Infrastructure.Auth](../Lewee.Infrastructure.Auth/README.md)** | Infrastructure | Provides `AuthTokenDelegatingHandler` for automatic authentication token propagation |

## Best Practices for DDD Infrastructure

1. **Service Discovery**: Use .NET Aspire service discovery rather than hardcoded URIs for infrastructure service location
2. **Distributed Tracing**: Leverage correlation IDs for tracing requests across infrastructure boundaries
3. **Bounded Context Separation**: Define focused Refit interfaces per bounded context or aggregate
4. **REST Conventions**: Follow REST principles in infrastructure interface definitions to align with domain semantics

## Troubleshooting Infrastructure Issues

### Authentication Infrastructure Issues

1. Ensure `AuthTokenDelegatingHandler` is configured (automatic with `AddWebApiHttpClient`)
2. Verify the user is authenticated with an `access_token` in the authentication context
3. Check `HttpContext` accessibility in your hosting model
4. For Blazor WebAssembly, authentication infrastructure requires different handling

### Correlation ID Infrastructure Issues

1. Verify `CorrelatingHttpMessageHandler` is configured (automatic with `AddWebApiHttpClient`)
2. Ensure Correlate middleware is configured in server infrastructure
3. Check that the server infrastructure echoes correlation IDs in responses

### Service Discovery Infrastructure Issues

1. Ensure your AppHost defines the infrastructure service with the correct name
2. Verify service discovery infrastructure is configured in your client application
3. Check that the infrastructure service is running and healthy

### Refit Infrastructure Serialization Issues

1. Ensure DTOs match the API contract (consider using shared contract assemblies)
2. Check JSON serialization settings are consistent across infrastructure boundaries
3. Use `[Body]` attribute for request bodies in infrastructure interfaces
4. Use `[Query]` attribute for query parameters in infrastructure interfaces
