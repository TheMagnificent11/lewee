using Lewee.Application.Mediation.Notifications;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public class OrderingDomainEventHandler : INotificationHandler<OrderStartedEvent>
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

    public async Task Handle(OrderStartedEvent notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Handling OrderStartedEvent for order {OrderId}",
            notification.OrderId);

        // Get the order with its pizzas to create the query projection
        var spec = new GetOrderQuerySpec(notification.OrderId);
        var order = await this.orderRepository.QueryOneAsync(spec, cancellationToken);

        if (order is null)
        {
            this.logger.LogError(
                "Order {OrderId} not found when handling OrderStartedEvent - this indicates a critical system error",
                notification.OrderId);
            return;
        }

        // Build the pizza DTOs with joined data
        var orderLines = order.Pizzas
            .Select(op => new OrderPizzaDto
            {
                Id = op.Id,
                PizzaId = op.PizzaId,
                PizzaName = op.Pizza.Name,
                PizzaPrice = op.Pizza.Price,
                Quantity = op.Quantity,
                LineTotal = op.Pizza.Price * op.Quantity,
            })
            .ToArray();

        var totalCost = orderLines.Sum(p => p.LineTotal);

        // Create the DTO for SignalR
        var dto = new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            StartedDateTime = order.StartedDateTime,
            SubmittedDateTime = order.SubmittedDateTime,
            PreparedDateTime = order.PreparedDateTime,
            CompletedDateTime = order.CompletedDateTime,
            DeliveryAddress = order.DeliveryAddress,
            Pizzas = orderLines,
            TotalCost = totalCost,
        };

        // Create or update the query projection
        var queryProjection = new OrderQueryProjection
        {
            CorrelationId = notification.CorrelationId,
            Order = dto,
        };

        await this.queryProjectionService.AddOrUpdateAsync(
            queryProjection,
            order.Id.ToString(),
            cancellationToken);

        // For anonymous users (demo app), send to all clients by passing null userId
        // In production with authentication, pass the actual order.UserId
        var clientEvent = new ClientEvent(notification.CorrelationId, userId: null, dto);

        await this.mediator.Publish(clientEvent, cancellationToken);

        this.logger.LogInformation(
            "Published OrderDto to SignalR for order {OrderId}",
            notification.OrderId);
    }
}
