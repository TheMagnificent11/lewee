using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class MenuItem : BaseAggregateRoot
{
    public MenuItem(Guid id, string name, decimal price, MenuItemType itemType)
        : base(id)
    {
        this.Name = name;
        this.Price = price;
        this.ItemTypeId = itemType;
    }

    // EF Constructor
    private MenuItem()
    {
        this.Name = string.Empty;
    }

    // Not really nullable but made nullable to prevent EF creating new enum table record
    public EnumEntity<MenuItemType>? ItemType { get; protected set; }
    public MenuItemType ItemTypeId { get; protected set; }

    public string Name { get; protected set; }
    public decimal Price { get; protected set; }

    internal static MenuItem EmptyMenuItem => new();
}
