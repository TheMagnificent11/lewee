using Lewee.Domain;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Domain;

public class Table : BaseEntity, IAggregateRoot
{
    public Table(int tableNumber)
    {
        this.TableNumber = tableNumber;
        this.StatusId = TableStatus.Unused;
        this.Status = new EnumEntity<TableStatus>(this.StatusId, this.StatusId.ToString());
    }

    public int TableNumber { get; protected set; }
    public TableStatus StatusId { get; protected set; }
    public EnumEntity<TableStatus> Status { get; protected set; }
}
