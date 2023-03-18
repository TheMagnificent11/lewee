namespace Sample.Restaurant.Domain;

public class MenuItemDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public static MenuItemDetails FromMenuItem(MenuItem menuItem)
    {
        return new MenuItemDetails
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Price = menuItem.Price,
        };
    }
}
