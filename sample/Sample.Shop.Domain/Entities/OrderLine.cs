using System.Collections.ObjectModel;
using Saji.Domain;

namespace Sample.Shop.Domain.Entities;

public class OrderLine : BaseEntity
{
    internal OrderLine(Order order, Product product, int quantity)
    {
        this.Order = order;
        this.Product = product;
        this.Quantity = quantity;

        this.OrderId = order.Id;
        this.ProductId = product.Id;
        this.Price = product.SalePrice;
        this.Cost = product.SalePrice * quantity;
    }

    // EF Constructor
    private OrderLine()
    {
        this.Order = new Order(Guid.Empty);
        this.Product = new Product("EF Constructor", "EF Constructor", 0, 1, Guid.Empty);
    }

    public Guid OrderId { get; protected set; }

    public Order Order { get; protected set; }

    public Guid ProductId { get; protected set; }

    public Product Product { get; protected set; }

    public int Quantity { get; protected set; }

    public decimal Price { get; protected set; }

    public decimal Cost { get; protected set; }
}
