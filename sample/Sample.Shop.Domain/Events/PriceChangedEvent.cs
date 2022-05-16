using Saji.Domain;

namespace Sample.Shop.Domain.Events;

public class PriceChangedEvent : IDomainEvent
{
    public PriceChangedEvent(Guid correlationId, Guid itemId, decimal newPrice)
    {
        this.CorrelationId = correlationId;
        this.ItemId = itemId;
        this.NewPrice = newPrice;
    }

    public Guid CorrelationId { get; }
    public Guid ItemId { get; }
    public decimal NewPrice { get; }
}
