using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;

namespace Lewee.Blazor.Tests.Integration;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction Map(object message, Guid correlationId)
    {
        return message switch
        {
            PizzaOrder order => new OrderCreatedAction(order, correlationId),
            _ => null
        };
    }
}

public record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;