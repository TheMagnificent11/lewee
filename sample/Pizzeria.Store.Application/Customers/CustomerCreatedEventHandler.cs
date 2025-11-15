using Lewee.Application.Mediation.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Customers;

public class CustomerCreatedEventHandler : INotificationHandler<CustomerCreatedEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<CustomerCreatedEventHandler> logger;

    public CustomerCreatedEventHandler(
        IMediator mediator,
        ILogger<CustomerCreatedEventHandler> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        this.logger.LogHandlingCustomerCreatedEvent(notification.CustomerId);

        var dto = new CustomerDto
        {
            Id = notification.CustomerId,
            ExternalId = notification.ExternalId,
        };

        // Send SignalR message to notify the web client
        var clientEvent = new ClientEvent(notification.CorrelationId, userId: null, dto);

        await this.mediator.Publish(clientEvent, cancellationToken);

        this.logger.LogPublishedCustomerDto(notification.CustomerId);
    }
}
