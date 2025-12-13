using Lewee.Application.Mediation.Notifications;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts.Orders;
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
        ArgumentNullException.ThrowIfNull(notification);

        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            { LoggingConsts.CorrelationId, notification.CorrelationId },
            { "OrderId", notification.OrderId },
            { "DomainEventName", nameof(OrderStartedEvent) },
        }))
        {
            this.logger.LogInformation("Started handling domain event");

            // Get the order with its pizzas to create the query projection
            var spec = new GetOrderQuerySpec(notification.OrderId);
            var order = await this.orderRepository.QueryOneAsync(spec, cancellationToken);

            if (order is null)
            {
                this.logger.LogError("Order not found");
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

            var clientEvent = new ClientEvent(notification.CorrelationId, notification.UserId, dto);

            await this.mediator.Publish(clientEvent, cancellationToken);

            this.logger.LogInformation("Completed handling domain event");
        }
    }
}
