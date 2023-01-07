using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class Table : BaseEntity, IAggregateRoot
{
    public Table(Guid id, int tableNumber)
        : base(id)
    {
        this.TableNumber = tableNumber;
    }

    public int TableNumber { get; protected set; }
    public bool IsInUse { get; protected set; }

    public void Use(Guid correlationId)
    {
        if (this.IsInUse)
        {
            throw new DomainException("Table is already being used");
        }

        this.IsInUse = true;
        this.DomainEvents.Raise(new TableInUseDomainEvent(correlationId, this.Id, this.TableNumber));
    }
}
