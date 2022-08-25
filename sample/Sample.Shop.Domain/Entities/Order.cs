using Saji.Domain;

namespace Sample.Shop.Domain.Entities;

public class Order : BaseEntity
{
    private readonly List<OrderLine> lines = new();

    public Order(Guid customerId)
    {
        this.CustomerId = customerId;

        this.StatusId = OrderStatus.ShoppingCart;
        this.OrderLines = this.lines;
    }

    // EF Constructor
    private Order()
    {
        this.OrderLines = this.lines;
    }

    public OrderStatus StatusId { get; protected set; }

    public EnumEntity<OrderStatus>? Status { get; protected set; }

    public Guid CustomerId { get; protected set; }

    public IReadOnlyCollection<OrderLine> OrderLines { get; protected set; }

    public decimal Total => this.lines.Sum(x => x.Cost);

    internal void AddToCart(Product product, int quantity)
    {
        this.lines.Add(new OrderLine(this, product, quantity));
    }
}
