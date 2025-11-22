using Lewee.Application.Mediation.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Customers;

public class CustomerCreatedEventHandler : INotificationHandler<UserCreatedEvent>
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

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        this.logger.LogHandlingCustomerCreatedEvent(notification.UserEntityId);

        var dto = new CustomerDto
        {
            Id = notification.UserEntityId,
            ExternalId = notification.ExternalId,
        };

        // Send SignalR message to notify the web client
        var clientEvent = new ClientEvent(notification.CorrelationId, userId: null, dto);

        await this.mediator.Publish(clientEvent, cancellationToken);

        this.logger.LogPublishedCustomerDto(notification.UserEntityId);
    }
}
