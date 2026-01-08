# Lewee.Infrastructure.HttpClient

This package provides HTTP client infrastructure for building applications that communicate with APIs, including type-safe REST clients, authentication token handling, correlation ID propagation, and structured API error handling.

## Dependencies

- [Refit](https://github.com/reactiveui/refit) - Type-safe REST client
- [Correlate](https://github.com/skwasjer/Correlate) - Correlation ID functionality
- [Lewee.Common](../Lewee.Common/README.md) - Shared utilities and constants

## Features

- **Refit API Client Support**: Easy configuration of type-safe API clients with service discovery
- **Authentication Token Handling**: Automatic Bearer token propagation for authenticated requests
- **Correlation ID Handling**: Automatic correlation ID propagation across HTTP requests using Correlate
- **API Error Handling**: Structured exception handling for API calls with NSwag compatibility

## Configuration

In the code below, `services` is `Microsoft.Extensions.DependencyInjection.IServiceCollection`.

### Adding Refit API Clients

Use `AddApiClient<T>` to configure type-safe Refit API clients with automatic authentication and correlation ID handling:

```cs
using Lewee.Infrastructure.HttpClient;

// Configure a Refit API client with service discovery
services.AddApiClient<IMyApiClient>("my-api-service");
```

This automatically configures:
- `AuthTokenDelegatingHandler` - Adds Bearer token from the current user's access token
- `CorrelatingHttpMessageHandler` (from Correlate) - Adds correlation ID header to all requests

### Service Discovery

The `AddApiClient<T>` method is designed to work with .NET Aspire service discovery. The `aspireApiServiceName` parameter should match the service name defined in your Aspire AppHost:

```cs
// In your AppHost
var api = builder.AddProject<Projects.MyApi>("my-api-service");

// In your client application
services.AddApiClient<IMyApiClient>("my-api-service");
```

## Usage

### Defining API Clients

Create a Refit interface for your API:

```cs
using Refit;

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

Register the client in your DI container:

```cs
services.AddApiClient<IOrdersApiClient>("orders-api");
```

### Using API Clients

Inject and use the API client in your services or components:

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

### API Error Handling

Use the `ApiException` extension methods to handle and log API errors:

```cs
using Lewee.Infrastructure.HttpClient;

try
{
    await apiClient.CreateOrderAsync(request);
}
catch (ApiException ex)
{
    // Log with appropriate level based on status code
    ex.Log(logger);

    // Handle the error appropriately
    throw;
}
```

The `Log` extension method automatically chooses the appropriate log level:
- **< 400**: Warning (unexpected response status)
- **400**: Information (bad request with response body)
- **401-499**: Information (client error)
- **500+**: Error (server error)

### Integration with Fluxor Effects

When using with Fluxor state management, handle API errors in your effects:

```cs
using Fluxor;
using Lewee.Infrastructure.HttpClient;

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
        catch (ApiException ex)
        {
            ex.Log(this.logger);
            dispatcher.Dispatch(new CreateOrderFailureAction
            {
                CorrelationId = action.CorrelationId,
                ErrorMessage = ex.Message
            });
        }
    }
}
```

## Components

| Component | Description |
|-----------|-------------|
| **[ApiCientConfiguration.cs](./ApiCientConfiguration.cs)** | Refit API client configuration with `AddApiClient<T>` |
| **[AuthTokenDelegatingHandler.cs](./AuthTokenDelegatingHandler.cs)** | Adds Bearer token from user's access token |
| **[ApiException.cs](./ApiException.cs)** | Exception class for API errors (NSwag-compatible) |
| **[ApiExceptionExtensions.cs](./ApiExceptionExtensions.cs)** | Helper methods to log API exceptions |
| **[ApiExceptionLogMessages.cs](./ApiExceptionLogMessages.cs)** | Source-generated log messages for API exceptions |

## Correlation ID Handling

This package uses [Correlate](https://github.com/skwasjer/Correlate) for correlation ID handling. The `CorrelatingHttpMessageHandler` automatically adds the `X-Correlation-ID` header to all outgoing HTTP requests.

The correlation ID flows through:

1. Incoming HTTP requests (via Correlate middleware on server)
2. Outgoing HTTP requests (via `CorrelatingHttpMessageHandler`)
3. Server-side logging
4. Response headers back to client

### Configuration

The `AddApiClient<T>` method automatically configures Correlate with the standard correlation header:

```cs
services.AddCorrelate(options => options.RequestHeaders = [RequestHeaders.CorrelationId]);
```

## Authentication Token Handling

The `AuthTokenDelegatingHandler` automatically:

1. Retrieves the `access_token` from the current `HttpContext`
2. Adds it as a `Bearer` token in the `Authorization` header
3. Logs warnings when tokens are missing

This is automatically configured when using `AddApiClient<T>`.

### Prerequisites

For authentication token handling to work:

1. The application must have `IHttpContextAccessor` registered
2. The user must be authenticated with an `access_token` claim
3. The `HttpContext` must be accessible (works with Blazor Server and server-side rendering)

## Integration with Other Lewee Packages

This package integrates with other Lewee packages:

| Package | Integration |
|---------|-------------|
| **[Lewee.Common](../Lewee.Common/README.md)** | Provides `RequestHeaders.CorrelationId` constant |
| **[Lewee.Blazor](../Lewee.Blazor/README.md)** | Uses this package for API client configuration in Blazor applications |
| **[Lewee.Infrastructure.AspNet](../Lewee.Infrastructure.AspNet/README.md)** | Server-side correlation ID handling |

## Best Practices

1. **Use service discovery with .NET Aspire**: Preferred over hardcoded URIs for production applications
2. **Handle API exceptions properly**: Always catch `ApiException` and use the `Log` extension method
3. **Use correlation IDs**: They enable distributed tracing across services
4. **Define focused API interfaces**: Create separate Refit interfaces for different API domains
5. **Use appropriate HTTP methods**: Follow REST conventions in your Refit interface definitions

## Troubleshooting

### Authentication Token Missing

1. Ensure `AuthTokenDelegatingHandler` is configured (automatic with `AddApiClient`)
2. Verify the user is authenticated and has an `access_token`
3. Check HttpContext accessibility in your hosting model
4. For Blazor WebAssembly, authentication tokens may need to be handled differently

### Correlation IDs Missing

1. Verify `CorrelatingHttpMessageHandler` is configured (automatic with `AddApiClient`)
2. Ensure Correlate middleware is configured on the server
3. Check that the server echoes correlation IDs in responses

### Service Discovery Not Working

1. Ensure your AppHost defines the service with the correct name
2. Verify service discovery is configured in your client application
3. Check that the service is running and healthy

### Refit Serialization Issues

1. Ensure your DTOs match the API contract
2. Check JSON serialization settings match between client and server
3. Use `[Body]` attribute for request bodies
4. Use `[Query]` attribute for query parameters
