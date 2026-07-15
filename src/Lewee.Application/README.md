# Lewee.Application

This package assists with building the application layer of an application using domain-driven design principles.

## Dependencies

- [FluentValidation](https://docs.fluentvalidation.net/en/latest)
- [MediatR](https://github.com/jbogard/MediatR)
- [Lewee.Domain](../Lewee.Domain/README.md)

## Configuration

To use this packages add the following lines to your service configuration of your application (`services` in the code below is `Microsoft.Extensions.DependencyInjection.ServicesCollection`).

```cs
services.AddApplication(typeof(ApplicationLayerClass).Assembly, typeof(DomainLayerClass).Assembly);
services.AddPipelineBehaviors();
```

The assembly parameters of `AddApplication` are used to configure `MediatR` (it will scan these assemblies for request and notification classes/handlers). In addition to this, the application layer assembly will by used to configure `FluentValidation` (it will use this assembly to scan for validators).

`AddPipelineBehaviors` adds the `MediatR` request pipelines behaviors listed in the [Pipeline Behaviors](#pipeline-behaviors) section. This only needs to be added once per application/solution; not for once each project that uses this package.

Additional behaviors can be added as parameters to `AddPipelineBehaviors`.

## Mediation

`MediatR` is used to assist with mediation, which allows the presentation layer of the application to have less dependencies e.g. API controller classes only need to inject the `IMediator` interface to handle web requests.

There a several interfaces that add a further layer of [command-query responsibility separation (CQRS)](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) on top of the `IRequest` interface in `MediatR`.

- [IQuery](./Mediation/Requests/IQuery.cs)
- [ICommand](./Mediation/Requests/ICommand.cs)
- [ITenantedRequest](./Mediation/Requests/ITenantRequest.cs)

### IQuery & ICommand

Both `IQuery` and `ICommand` implement [IApplicationRequest](./Mediation/Requests/IApplicationRequest.cs), which has a `CorrelationId` `Guid` property.

As mentioned in the [Pipeline Behaviors](#pipeline-behaviors) section, the `CorrelationId` property is used in the `CorelationIdLoggingBehavior` to decorate the logs with the correlation ID for every `MediatR` request that implements `IApplicationRequest`.

Query handlers that handle `IQuery` input types are required to have [QueryResult](./Mediation/Requests/QueryResult.cs) as the return type. `QueryResult` has a type parameter that is used to specify the query data type.

Command handlers that handle `ICommand` input types are required to return a [CommandResult](./Mediation/Requests/CommandResult.cs).

Both `QueryResult` and `CommandResult` inherit from [Result](./Mediation/Requests/Result.cs), which has the following properties.

- `IsSuccess`
  - Whether the request was successful.
- `Status`
  - An [ResultStatus](./Mediation/Requests/ResultStatus.cs) enum used to specify the type of result.
- `Errors`
  - A dictionary keyed by request property for any errors encountered

#### Sample Code

[Query](../../sample/Pizzeria.Store.Application/Pizzas/GetPizzasQuery.cs)

[Command](../../sample/Pizzeria.Store.Application/Orders/StartOrderCommand.cs)

### ITenantRequest

`ITenantedRequest` has a `TenantId` `Guid` property used to specify the tenant ID for multi-tenanted applications.

As mentioned in the [Pipeline Behaviors](#pipeline-behaviors) section, the `TenantId` property is used in the `TenantLoggingBehavior` to decorate the logs with tenant ID for every `MediatR` request that implements `ITenantedRequest`.

### Pipeline Behaviors

- [CorelationIdLoggingBehavior](./Mediation//Behaviors/CorrelationIdLoggingBehavior.cs)
  - Adds a `CorrelationId` structured logging property for every request. Uses `CorrelationId` property if the request implements `IApplicationRequest`, otherwise it generates a new GUID.
- [DomainExceptionBehavior](./Mediation/Behaviors/DomainExceptionBehavior.cs)
  - Catches [DomainException](../Lewee.Domain/DomainException.cs), which is defined in the [Lewee.Domain](../Lewee.Domain/README.md) package, and returns a failure result.
- [FailureLoggingBehavior](./Mediation/Behaviors/FailureLoggingBehavior.cs)
  - Checks the `Status` of requests that have `Result` return types and logs an error if there was an unexpected failure, or an information log if there was domain/validation failure.
- [PerformanceLoggingBehavior](./Mediation/Behaviors/PerformanceBehavior.cs)
  - Logs the time taken to execute the request using high-performance logging.
- [TenantLoggingBehavior](./Mediation/Behaviors/TenantLoggingBehavior.cs)
  - Adds a `TenantId` structured logging property for every request that implements `ITenantedRequest`.
- [UnhandledExceptionBehavior](./Mediation//Behaviors/CorrelationIdLoggingBehavior.cs)
  - Catches unhandled exceptions and logs them as errors
- [ValidationBehavior](./Mediation/Behaviors/ValidationBehavior.cs)
  - Executes `FluentValidation` validators for the request and returns a `CommandResult` with the errors dictionary populated if there are validation errors found, otherwise the rest of the pipeline is executed.

### ClientEvent Notification

The [ClientEvent](./Mediation/Notifications/ClientEvent.cs) can be published via `MediatR.IMediator.Publish` to broadcast an event to connected Blazor clients. The `ClientEvent` is handled by infrastructure in the [Lewee.Infrastructure.ServerEvents](../Lewee.Infrastructure.ServerEvents/README.md) package, which writes the event to an SSE channel for streaming to clients.

For the complete server-sent events infrastructure implementation, see the [Lewee.Infrastructure.ServerEvents](../Lewee.Infrastructure.ServerEvents/README.md) package.

#### Sample

*Note: The current Pizzeria sample application demonstrates the ClientEvent pattern with the `StartOrderCompletedAction` being dispatched when an order is created.*
