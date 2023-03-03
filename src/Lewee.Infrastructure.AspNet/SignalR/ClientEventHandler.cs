using Lewee.Application.Mediation.Notifications;
using MediatR;

namespace Lewee.Infrastructure.AspNet.SignalR;

internal class ClientEventHandler<T> : INotificationHandler<ClientEvent<T>>
    where T : class
{
    public Task Handle(ClientEvent<T> notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
