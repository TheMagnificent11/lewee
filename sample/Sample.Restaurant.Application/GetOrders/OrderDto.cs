namespace Sample.Restaurant.Application.GetOrders;

public class OrderDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
}
