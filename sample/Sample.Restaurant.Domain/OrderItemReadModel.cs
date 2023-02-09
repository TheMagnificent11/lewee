namespace Sample.Restaurant.Domain;

public class OrderItemReadModel
{
    public MenuItemReadModel MenuItem { get; set; } = new();
    public int Quantity { get; set; }
}
