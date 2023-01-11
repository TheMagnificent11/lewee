using Lewee.Domain;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Domain;

public class Order : BaseEntity
{
    internal Order(Table table, Guid correlationId)
    {
        this.TableId = table.Id;
        this.Table = table;
        this.OrderStatusId = Contracts.OrderStatus.Ordering;
        this.OrderStatus = new EnumEntity<OrderStatus>(this.OrderStatusId);

        this.DomainEvents.Raise(new OrderCreatedDomainEvent(
            correlationId,
            this.Id,
            table.Id,
            table.TableNumber,
            DateTime.UtcNow));
    }

    // EF Constructor
    private Order()
    {
        this.TableId = Guid.Empty;
        this.Table = new Table(this.TableId, 0);
        this.OrderStatusId = Contracts.OrderStatus.Ordering;
        this.OrderStatus = new EnumEntity<OrderStatus>(this.OrderStatusId);
    }

    public Guid TableId { get; protected set; }
    public Table Table { get; protected set; }
    public OrderStatus OrderStatusId { get; protected set; }
    public EnumEntity<OrderStatus> OrderStatus { get; protected set; }
}
