using Lewee.Application.Mediation.Notifications;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public class OrderingDomainEventHandler :
    INotificationHandler<OrderStartedEvent>,
    INotificationHandler<PickupOrderSubmittedEvent>,
    INotificationHandler<DeliveryOrderSubmittedEvent>,
    INotificationHandler<OrderMakingStartedEvent>,
    INotificationHandler<OrderPreparedEvent>,
    INotificationHandler<OrderOutForDeliveryEvent>,
    INotificationHandler<OrderCompletedEvent>
{
    private readonly IRepository<Order> orderRepository;
    private readonly IQueryProjectionService queryProjectionService;
    private readonly IMediator mediator;
    private readonly ILogger<OrderingDomainEventHandler> logger;

    public OrderingDomainEventHandler(
        IRepository<Order> orderRepository,
        IQueryProjectionService queryProjectionService,
        IMediator mediator,
        ILogger<OrderingDomainEventHandler> logger)
    {
        this.orderRepository = orderRepository;
        this.queryProjectionService = queryProjectionService;
        this.mediator = mediator;
        this.logger = logger;
    }

    public Task Handle(OrderStartedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(OrderStartedEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    public Task Handle(PickupOrderSubmittedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(PickupOrderSubmittedEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    public Task Handle(DeliveryOrderSubmittedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(DeliveryOrderSubmittedEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    public Task Handle(OrderMakingStartedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(OrderMakingStartedEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    public Task Handle(OrderPreparedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(OrderPreparedEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    public Task Handle(OrderOutForDeliveryEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(OrderOutForDeliveryEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    public Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return this.ProjectAndPublishAsync(
            nameof(OrderCompletedEvent),
            notification.OrderId,
            notification.CorrelationId,
            notification.UserId,
            cancellationToken);
    }

    private async Task ProjectAndPublishAsync(
        string domainEventName,
        Guid orderId,
        Guid correlationId,
        string? userId,
        CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            { LoggingConsts.CorrelationId, correlationId },
            { "OrderId", orderId },
            { "DomainEventName", domainEventName },
        }))
        {
            this.logger.LogInformation("Started handling domain event");

            var spec = new GetOrderQuerySpec(orderId);
            var order = await this.orderRepository.QueryOneAsync(spec, cancellationToken);

            if (order is null)
            {
                this.logger.LogError("Order not found");
                return;
            }

            var dto = OrderDtoFactory.Create(order);

            var queryProjection = new OrderQueryProjection
            {
                CorrelationId = correlationId,
                Order = dto,
            };

            await this.queryProjectionService.AddOrUpdateAsync(
                queryProjection,
                order.Id.ToString(),
                cancellationToken);

            var clientEvent = new ClientEvent(correlationId, userId, dto);

            await this.mediator.Publish(clientEvent, cancellationToken);

            this.logger.LogInformation("Completed handling domain event");
        }
    }
}
