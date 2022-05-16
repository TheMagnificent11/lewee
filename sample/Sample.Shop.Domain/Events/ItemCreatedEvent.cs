using Saji.Domain;
using Sample.Shop.Domain.Entities;

namespace Sample.Shop.Domain.Events;

public class ItemCreatedEvent : IDomainEvent
{
    public ItemCreatedEvent(Guid correlationId, Item item)
    {
        this.CorrelationId = correlationId;
        this.ItemId = item.Id;
        this.Name = item.Name;
        this.Description = item.Description;
        this.SalePrice = item.SalePrice;
    }

    public Guid CorrelationId { get; }
    public Guid ItemId { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal SalePrice { get; }
}
