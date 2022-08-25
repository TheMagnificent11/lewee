using Saji.Domain;

namespace Sample.Shop.Domain.Entities;

public class Order : BaseEntity
{
    private readonly List<OrderLine> cart = new();

    public Order(Guid customerId)
    {
        this.CustomerId = customerId;

        this.Status = OrderStatus.ShoppingCart;
        this.ShopptingCart = this.cart;
    }

    // EF Constructor
    private Order()
    {
        this.ShopptingCart = this.cart;
    }

    public OrderStatus Status { get; protected set; }

    public Guid CustomerId { get; protected set; }

    public IReadOnlyCollection<OrderLine> ShopptingCart { get; protected set; }

    public decimal Total => this.cart.Sum(x => x.Cost);

    internal void AddToCart(Product product, int quantity)
    {
        this.cart.Add(new OrderLine(product, quantity));
    }
}
