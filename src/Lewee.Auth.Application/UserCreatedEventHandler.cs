using Lewee.Application.Mediation.Notifications;
using Lewee.Auth.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

/// <summary>
/// Publishes newly created users to clients.
/// </summary>
public sealed class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<UserCreatedEventHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCreatedEventHandler"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="logger">Logger.</param>
    public UserCreatedEventHandler(IMediator mediator, ILogger<UserCreatedEventHandler> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        this.logger.LogHandlingUserCreatedEvent(notification.UserEntityId);
        var dto = new UserDto
        {
            Id = notification.UserEntityId,
            ExternalId = notification.ExternalId,
        };

        await this.mediator.Publish(
            new ClientEvent(notification.CorrelationId, notification.ExternalId, dto),
            cancellationToken);
        this.logger.LogPublishedUserDto(notification.UserEntityId);
    }
}
