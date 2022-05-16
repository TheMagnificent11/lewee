using Saji.Domain;

namespace Sample.Shop.Domain.Events;

public class PriceChangedEvent : IDomainEvent
{
    public PriceChangedEvent(Guid correlationId, Guid catalogueItemId, decimal newPrice)
    {
        this.CorrelationId = correlationId;
        this.CatalogueItemId = catalogueItemId;
        this.NewPrice = newPrice;
    }

    public Guid CorrelationId { get; }
    public Guid CatalogueItemId { get; }
    public decimal NewPrice { get; }
}
