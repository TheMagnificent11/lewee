using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;

namespace Pizzeria.Store.WebClient.States;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        // TODO: Add SignalR message mapping when needed
        return null;
    }
}
