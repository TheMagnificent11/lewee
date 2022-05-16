using Saji.Domain;

namespace Sample.Shop.Domain.Events;

public class StockAddedEvent : IDomainEvent
{
    public StockAddedEvent(Guid correlationId, Guid catalogueItemId, int quantity, int newStockLevel)
    {
        this.CorrelationId = correlationId;
        this.CatalogueItemId = catalogueItemId;
        this.Quantity = quantity;
        this.NewStockLevel = newStockLevel;
    }

    public Guid CorrelationId { get; }
    public Guid CatalogueItemId { get; }
    public int Quantity { get; }
    public int NewStockLevel { get; }
}
