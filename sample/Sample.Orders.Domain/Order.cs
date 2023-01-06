using Lewee.Domain;

namespace Sample.Orders.Domain;
public class Order : BaseEntity, IAggregateRoot
{
    private readonly List<MenuItem> items;

    public Order(int tableNumber)
    {
        this.items = new List<MenuItem>();

        this.TableNumber = tableNumber;
        this.Items = this.items;
    }

    public int TableNumber { get; protected set;  }
    public IReadOnlyCollection<MenuItem> Items { get; protected set; }
    public int ItemCount => this.items.Count;
    public decimal Total => this.items.Sum(x => x.Price);

    public void AddItem(MenuItem item)
    {
        this.items.Add(item);
    }
}
