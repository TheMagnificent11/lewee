using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class Table : BaseEntity, IAggregateRoot
{
    private readonly List<Order> orders = new();

    public Table(Guid id, int tableNumber)
        : base(id)
    {
        this.TableNumber = tableNumber;
    }

    public Order? CurrentOrder => this.orders
        .Where(x => x.OrderStatusId != Contracts.OrderStatus.Paid)
        .Where(x => !x.IsDeleted)
        .OrderByDescending(x => x.CreatedAtUtc)
        .FirstOrDefault();

    public int TableNumber { get; protected set; }
    public bool IsInUse { get; protected set; }
    public IReadOnlyCollection<Order> Orders => this.orders;

    internal static Table EmptyTable => new(Guid.Empty, 0);

    public void Use(Guid correlationId)
    {
        if (this.IsInUse)
        {
            throw new DomainException("Table is already being used");
        }

        this.IsInUse = true;

        this.orders.Add(Order.StartNewOrder(this, correlationId));

        this.DomainEvents.Raise(new TableInUseDomainEvent(correlationId, this.Id, this.TableNumber));
    }
}
