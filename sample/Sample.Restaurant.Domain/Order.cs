using Lewee.Domain;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Domain;

public class Order : BaseEntity
{
    internal Order(Guid tableId, int tableNumber, Guid correlationId)
    {
        this.TableId = tableId;
        this.Table = new Table(tableId, tableNumber);
        this.OrderStatusId = Contracts.OrderStatus.Ordering;
        this.OrderStatus = new EnumEntity<OrderStatus>(this.OrderStatusId);

        this.DomainEvents.Raise(new OrderCreatedDomainEvent(
            correlationId,
            this.Id,
            tableId,
            tableNumber,
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
