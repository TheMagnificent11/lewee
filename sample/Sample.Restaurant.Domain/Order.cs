using Lewee.Domain;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Domain;

public class Order : BaseEntity
{
    private readonly List<OrderItem> items = new();

    private Order(Table table)
        : base(Guid.Empty)
    {
        this.TableId = table.Id;
        this.Table = table;
        this.OrderStatusId = Contracts.OrderStatus.Ordering;
    }

    // EF Constructor
    private Order()
    {
        this.TableId = Guid.Empty;
        this.Table = new Table(this.TableId, 0);
    }

    public Guid TableId { get; protected set; }
    public Table Table { get; protected set; }
    public OrderStatus OrderStatusId { get; protected set; }
    public EnumEntity<OrderStatus>? OrderStatus { get; protected set; } // Not really nullable, but this is difficult to set for EF
    public IReadOnlyCollection<OrderItem> Items => this.items;
    public decimal Total => this.items.Sum(x => x.ItemTotal);

    internal static Order EmptyOrder => new(Table.EmptyTable);

    public static Order StartNewOrder(Table table, Guid correlationId)
    {
        var order = new Order(table);

        order.DomainEvents.Raise(new OrderCreatedDomainEvent(
            correlationId,
            order.Id,
            table.Id,
            table.TableNumber,
            DateTime.UtcNow));

        return order;
    }

    public void AddItem(MenuItem menuItem, int quantity, Guid correlationId)
    {
        this.items.Add(OrderItem.AddToOrder(this, menuItem, quantity, correlationId));
    }
}
