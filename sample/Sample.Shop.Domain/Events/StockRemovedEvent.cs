using Saji.Domain;

namespace Sample.Shop.Domain.Events;

public class StockRemovedEvent : IDomainEvent
{
    public StockRemovedEvent(Guid correlationId, Guid catalogueItemId, int quantity, int newStockLevel)
    {
        this.CorrelationId = correlationId;
        this.ItemId = catalogueItemId;
        this.Quantity = quantity;
        this.NewStockLevel = newStockLevel;
    }

    public Guid CorrelationId { get; }
    public Guid ItemId { get; }
    public int Quantity { get; }
    public int NewStockLevel { get; }
}
