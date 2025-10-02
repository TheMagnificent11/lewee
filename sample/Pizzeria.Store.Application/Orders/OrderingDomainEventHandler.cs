using System.Text.Json;
using FreeMediator;
using Lewee.Application.Mediation.Notifications;
using Lewee.Domain;
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
            this.logger.LogWarning(
                "Order {OrderId} not found when handling OrderStartedEvent",
                notification.OrderId);
            return;
        }

        // Build the pizza DTOs with joined data
        var pizzaDtos = order.Pizzas.Select(op => new OrderPizzaDto
        {
            Id = op.Id,
            PizzaId = op.PizzaId,
            PizzaName = op.Pizza.Name,
            PizzaPrice = op.Pizza.Price,
            Quantity = op.Quantity,
            LineTotal = op.Pizza.Price * op.Quantity
        }).ToArray();

        var totalCost = pizzaDtos.Sum(p => p.LineTotal);

        // Create or update the query projection
        var queryProjection = new OrderQueryProjection
        {
            CorrelationId = notification.CorrelationId,
            UserId = order.UserId,
            StartedDateTime = order.StartedDateTime,
            SubmittedDateTime = order.SubmittedDateTime,
            PreparedDateTime = order.PreparedDateTime,
            CompletedDateTime = order.CompletedDateTime,
            DeliveryAddress = order.DeliveryAddress,
            PizzasJson = JsonSerializer.Serialize(pizzaDtos),
            TotalCost = totalCost
        };

        await this.queryProjectionService.AddOrUpdateAsync(
            queryProjection,
            order.Id.ToString(),
            cancellationToken);

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
            Pizzas = pizzaDtos,
            TotalCost = totalCost
        };

        var clientEvent = new ClientEvent(notification.CorrelationId, notification.UserId, dto);

        await this.mediator.Publish(clientEvent, cancellationToken);

        this.logger.LogInformation(
            "Published OrderDto to SignalR for order {OrderId}",
            notification.OrderId);
    }
}
