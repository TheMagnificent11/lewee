using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record StartOrderCompletedAction : IMessageReceivedAction
{
    public StartOrderCompletedAction(Guid orderId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
}
