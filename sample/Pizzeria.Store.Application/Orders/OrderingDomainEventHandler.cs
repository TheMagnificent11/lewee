using FreeMediator;
using Lewee.Application.Mediation.Notifications;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public class OrderingDomainEventHandler : INotificationHandler<OrderStartedEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<OrderingDomainEventHandler> logger;

    public OrderingDomainEventHandler(
        IMediator mediator,
        ILogger<OrderingDomainEventHandler> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(OrderStartedEvent notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Handling OrderStartedEvent for order {OrderId}",
            notification.OrderId);

        var dto = new OrderStartedEventDto
        {
            OrderId = notification.OrderId,
            UserId = notification.UserId ?? string.Empty,
            StartedDateTime = notification.StartedDateTime
        };

        var clientEvent = new ClientEvent(notification.CorrelationId, notification.UserId, dto);

        await this.mediator.Publish(clientEvent, cancellationToken);

        this.logger.LogInformation(
            "Published OrderStartedEventDto to SignalR for order {OrderId}",
            notification.OrderId);
    }
}
