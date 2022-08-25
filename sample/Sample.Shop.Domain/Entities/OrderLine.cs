namespace Sample.Shop.Domain.Entities;

public class OrderLine
{
    internal OrderLine(Product product, int quantity)
    {
        this.Product = product;
        this.Quantity = quantity;

        this.ProductId = product.Id;
        this.Cost = product.SalePrice * quantity;
    }

    // EF Constructor
    private OrderLine()
    {
        this.Product = new Product("EF Constructor", "EF Constructor", 0, 1, Guid.NewGuid());
    }

    public Guid ProductId { get; protected set; }

    public Product Product { get; protected set; }

    public int Quantity { get; protected set; }

    public decimal Cost { get; protected set; }
}
