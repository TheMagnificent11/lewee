using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;

namespace Pizzeria.Store.Web.Infrastructure;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        throw new NotSupportedException();
    }
}
