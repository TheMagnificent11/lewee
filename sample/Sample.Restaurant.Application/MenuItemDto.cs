namespace Sample.Restaurant.Application;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PriceLabel => $"{this.Price:0.00}";
}
