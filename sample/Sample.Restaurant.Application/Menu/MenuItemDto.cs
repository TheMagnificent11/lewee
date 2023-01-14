namespace Sample.Restaurant.Application.Menu;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PriceLabel => $"{this.Price.ToString("0.00")}";
}
