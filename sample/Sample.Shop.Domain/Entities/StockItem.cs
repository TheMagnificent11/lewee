using Saji.Domain;
using Sample.Shop.Domain.Events;

namespace Sample.Shop.Domain.Entities;

public class StockItem : BaseEntity
{
    public StockItem(Guid correlationId, string name, string description, decimal salePrice, int stockLevel)
    {
        this.Name = name;
        this.Description = description;
        this.SalePrice = salePrice;
        this.StockLevel = stockLevel;

        this.DomainEvents.Raise(new StockItemCreatedEvent(correlationId, this));
    }

    // EF Constructor
    private StockItem()
    {
        this.Name = string.Empty;
        this.Description = string.Empty;
    }

    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public decimal SalePrice { get; protected set; }
    public int StockLevel { get; protected set; }

    public bool InStock => this.StockLevel > 0;

    public void AddStock(int quantity, Guid correlationId)
    {
        this.StockLevel += quantity;

        this.DomainEvents.Raise(new StockAddedEvent(correlationId, this.Id, quantity, this.StockLevel));
    }

    public void Sell(int quantity, Guid correlationId)
    {
        this.StockLevel -= quantity;

        this.DomainEvents.Raise(new StockRemovedEvent(correlationId, this.Id, quantity, this.StockLevel));
    }

    public void ChangeSalePrice(decimal newPrice, Guid correlationId)
    {
        this.SalePrice = newPrice;

        this.DomainEvents.Raise(new PriceChangedEvent(correlationId, this.Id, this.SalePrice));
    }
}
