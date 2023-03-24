namespace Sample.Restaurant.Application;

public class OrderItemDto
{
    public MenuItemDto MenuItem { get; set; } = new();
    public int Quantity { get; set; }
}
