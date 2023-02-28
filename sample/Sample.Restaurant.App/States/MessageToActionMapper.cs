using Lewee.Blazor.Events;
using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.App.States;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message)
    {
        throw new NotImplementedException();
    }
}
