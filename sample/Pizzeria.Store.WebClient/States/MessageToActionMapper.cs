using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.States;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        return message switch
        {
            OrderStartedEventDto orderStarted => new StartOrderCompletedAction(orderStarted.OrderId, correlationId),
            _ => null
        };
    }
}
