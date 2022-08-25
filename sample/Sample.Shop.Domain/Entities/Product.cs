using System.Collections.ObjectModel;
using Saji.Domain;
using Sample.Shop.Domain.Events;

namespace Sample.Shop.Domain.Entities;

public class Product : BaseEntity
{
    private readonly List<OrderLine> orders = new();

    public Product(string name, string? description, decimal salePrice, int stockLevel, Guid correlationId)
    {
        this.Name = name;
        this.Description = description;
        this.SalePrice = salePrice;
        this.StockLevel = stockLevel;

        this.OrdersLines = this.orders;

        this.DomainEvents.Raise(new ProductCreatedEvent(correlationId, this));
    }

    // EF Constructor
    private Product()
    {
        this.Name = string.Empty;
        this.OrdersLines = this.orders;
    }

    public string Name { get; protected set; }
    public string? Description { get; protected set; }
    public decimal SalePrice { get; protected set; }
    public int StockLevel { get; protected set; }

    public IReadOnlyCollection<OrderLine> OrdersLines { get; protected set; }

    public bool InStock => this.StockLevel > 0;

    public void AddStock(int quantity, Guid correlationId)
    {
        this.StockLevel += quantity;

        this.DomainEvents.Raise(new StockAddedEvent(correlationId, this.Id, quantity, this.StockLevel));
    }

    public void AddToOrder(Order order, int quantity)
    {
        order.AddToCart(this, quantity);

        // TODO: reduce stock levels checkout is complete
        ////this.StockLevel -= quantity;
        ////this.DomainEvents.Raise(new StockRemovedEvent(correlationId, this.Id, quantity, this.StockLevel));
    }

    public void ChangeSalePrice(decimal newPrice, Guid correlationId)
    {
        this.SalePrice = newPrice;

        this.DomainEvents.Raise(new PriceChangedEvent(correlationId, this.Id, this.SalePrice));
    }

    public static class MaxLengths
    {
        public const int Name = 100;
        public const int Description = 1000;
    }
}
