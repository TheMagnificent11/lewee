using Lewee.Domain;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Domain;

public class Order : BaseEntity
{
    internal Order(Guid tableId, int tableNumber)
    {
        this.TableId = tableId;
        this.Table = new Table(tableId, tableNumber);
        this.OrderStatusId = Contracts.OrderStatus.Ordering;
        this.OrderStatus = new EnumEntity<OrderStatus>(this.OrderStatusId);
    }

    public Guid TableId { get; protected set; }
    public Table Table { get; protected set; }
    public OrderStatus OrderStatusId { get; protected set; }
    public EnumEntity<OrderStatus> OrderStatus { get; protected set; }
}
