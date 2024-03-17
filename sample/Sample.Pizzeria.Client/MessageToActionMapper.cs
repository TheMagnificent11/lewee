using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;

namespace Sample.Pizzeria.Client;

public sealed class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        return null;
    }
}
