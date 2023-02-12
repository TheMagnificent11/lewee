namespace Sample.Restaurant.Domain;

public class OrderItemDetails
{
    public MenuItemDetails MenuItem { get; set; } = new();
    public int Quantity { get; set; }

    public static OrderItemDetails FromMenuItem(MenuItem menuItem)
    {
        return new OrderItemDetails
        {
            MenuItem = MenuItemDetails.FromMenuItem(menuItem),
        };
    }
}
