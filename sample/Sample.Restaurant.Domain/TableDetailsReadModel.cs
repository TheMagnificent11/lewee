using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class TableDetailsReadModel : IReadModel
{
    public Guid CorrelationId { get; set; }
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public bool IsInUse { get; set; }
    public List<OrderItemReadModel> Items { get; set; } = new();
}
