using Saji.Domain;

namespace Sample.Shop.Domain.Events;

public class PriceChangedEvent : IDomainEvent
{
    public PriceChangedEvent(Guid correlationId, Guid productId, decimal newPrice)
    {
        this.CorrelationId = correlationId;
        this.ProductId = productId;
        this.NewPrice = newPrice;
    }

    public Guid CorrelationId { get; }
    public Guid ProductId { get; }
    public decimal NewPrice { get; }
}
