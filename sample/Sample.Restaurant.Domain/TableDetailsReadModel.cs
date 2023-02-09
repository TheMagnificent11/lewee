namespace Sample.Restaurant.Domain;

public class TableDetailsReadModel
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public bool IsInUse { get; set; }
    public OrderItemReadModel[] Items { get; set; } = Array.Empty<OrderItemReadModel>();
}
