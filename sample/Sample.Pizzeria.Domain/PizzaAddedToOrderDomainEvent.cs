using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class PizzaAddedToOrderDomainEvent : DomainEvent
{
    public PizzaAddedToOrderDomainEvent(Guid orderId, Guid orderPizzaId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.OrderPizzaId = orderPizzaId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
    public Guid OrderPizzaId { get; }
}
