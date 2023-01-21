namespace Sample.Restaurant.Application;

public class OrderItemDto
{
    public MenuItemDto MenuItem { get; set; } = new MenuItemDto();
    public int Quantity { get; set; }
}
