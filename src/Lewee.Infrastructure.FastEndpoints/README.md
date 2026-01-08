# Lewee.Infrastructure.FastEndpoints

FastEndpoints base classes for building Web API endpoints with MediatR integration and correlation ID support.

## Purpose

This package provides abstract base classes for building FastEndpoints-based Web API endpoints that integrate with the CQRS pattern implemented in `Lewee.Application`. It simplifies endpoint creation by providing common functionality for command and query handling, result mapping, and correlation ID propagation.

## Dependencies

- [FastEndpoints](https://fast-endpoints.com)
- [Lewee.Application](../Lewee.Application/Lewee.Application.csproj) - CQRS implementation
- [Lewee.Infrastructure.Correlate](../Lewee.Infrastructure.Correlate/Lewee.Infrastructure.Correlate.csproj) - Correlation ID support

## Components

### CommandEndpoint\<TRequest>

Abstract base class for endpoints that handle commands (POST, PUT, PATCH, DELETE operations):

```cs
public class CreateOrderEndpoint : CommandEndpoint<CreateOrderRequest>
{
    protected override string Route => "/orders";
    protected override string Name => "CreateOrder";
    protected override CommandType CommandType => CommandType.Post;
    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        var command = new CreateOrderCommand(this.CorrelationId, req.CustomerId, req.Items);
        var result = await this.Mediator.Send(command, ct);
        await this.ToResponseAsync(result, ct);
    }
}
```

**Abstract Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `Route` | `string` | The endpoint route |
| `Name` | `string` | The endpoint name for OpenAPI |
| `CommandType` | `CommandType` | HTTP method (Post, Put, Patch, Delete) |
| `IsAnonymousAllowed` | `bool` | Whether anonymous access is allowed |

**Protected Members:**

| Member | Type | Description |
|--------|------|-------------|
| `Mediator` | `IMediator` | MediatR mediator instance |
| `CorrelationId` | `Guid` | Current request correlation ID |

### QueryEndpoint\<T>

Abstract base class for endpoints that handle queries (GET operations):

```cs
public class GetOrdersEndpoint : QueryEndpoint<OrderDto[]>
{
    protected override string Route => "/orders";
    protected override string Name => "GetOrders";
    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetOrdersQuery(this.CorrelationId);
        var result = await this.Mediator.Send(query, ct);
        await this.ToResponseAsync(result, ct);
    }
}
```

**Abstract Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `Route` | `string` | The endpoint route |
| `Name` | `string` | The endpoint name for OpenAPI |
| `IsAnonymousAllowed` | `bool` | Whether anonymous access is allowed |

### CommandType

Enum for specifying the HTTP method of command endpoints:

| Value | HTTP Method |
|-------|-------------|
| `Post` | POST |
| `Put` | PUT |
| `Patch` | PATCH |
| `Delete` | DELETE |

### Result Handling

Both endpoint base classes provide `ToResponseAsync` methods that map `Lewee.Application` result types to HTTP responses:

| Result Status | HTTP Response |
|---------------|---------------|
| `Success` | 200 OK |
| `NotFound` | 404 Not Found |
| `ValidationError` | 400 Bad Request |
| `DomainError` | 400 Bad Request |
| `Unauthenticated` | 401 Unauthorized |
| `Forbidden` | 403 Forbidden |
| `Conflict` | 409 Conflict |
| `UnexpectedError` | 500 Internal Server Error |

### CorrelationContextAccessorExtensions

Internal extension method to get correlation ID from `ICorrelationContextAccessor`, generating a new GUID if not present.

## Sample Usage

See the [Pizzeria Store API project](../../sample/Pizzeria.Store.Api/) for complete implementation examples of command and query endpoints.
