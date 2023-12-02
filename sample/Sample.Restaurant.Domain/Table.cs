using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class Table : AggregateRoot
{
    private readonly List<Order> orders = new();

    public Table(Guid id, int tableNumber)
        : base(id)
    {
        this.TableNumber = tableNumber;
    }

    public static Table[] DefaultData => GenerateDefaultData();

    public Order? CurrentOrder => this.orders
        .Where(x => x.OrderStatusId != OrderStatus.Paid)
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
            throw new DomainException(ErrorMessages.TableInUse);
        }

        this.IsInUse = true;

        this.orders.Add(Order.StartNewOrder(this));

        this.DomainEvents.Raise(new TableInUseDomainEvent(correlationId, this.Id, this.TableNumber));
    }

    public void OrderMenuItem(MenuItem menuItem, Guid correlationId)
    {
        if (!this.IsInUse || this.CurrentOrder == null)
        {
            throw new DomainException(ErrorMessages.CannotOrderIfTableNotInUse);
        }

        this.CurrentOrder.AddItem(menuItem);

        this.DomainEvents.Raise(new OrderItemAddedDomainEvent(
            correlationId,
            this.Id,
            this.TableNumber,
            this.CurrentOrder.Id,
            menuItem.Id,
            menuItem.Price));
    }

    public void RemovedMenuItem(MenuItem menuItem, Guid correlationId)
    {
        if (!this.IsInUse || this.CurrentOrder == null)
        {
            throw new DomainException(ErrorMessages.CannotOrderIfTableNotInUse);
        }

        this.CurrentOrder.RemoveItem(menuItem);

        this.DomainEvents.Raise(new OrderItemRemovedDomainEvent(
            correlationId,
            this.Id,
            this.TableNumber,
            this.CurrentOrder.Id,
            menuItem.Id,
            menuItem.Price));
    }

    private static Table[] GenerateDefaultData()
    {
        var ids = new List<Guid>
            {
                new("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"),
                new("ec0fad21-c060-4315-a39f-6947deccd8fa"),
                new("735e6fee-be38-4b02-a1e9-659e727c072e"),
                new("45694a13-30c1-4ff9-b7d2-8079657a6e29"),
                new("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"),
                new("2a150b61-8f9a-497f-a77a-2c701158b5a5"),
                new("830f9c25-7cbd-44b7-84b1-bd55973deca9"),
                new("3cd0f023-94e8-4114-b139-21d4955e1bab"),
                new("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"),
                new("a6ce6962-20ec-4def-be6d-d568a12a022c")
            };

        var data = new List<Table>();

        var tableNumber = 1;
        ids.ForEach(x => data.Add(new Table(x, tableNumber++)));

        return data.ToArray();
    }

    public static class ErrorMessages
    {
        public const string TableInUse = "Table is already being used";
        public const string CannotOrderIfTableNotInUse = "Cannot order items if table is not in use";
    }
}
