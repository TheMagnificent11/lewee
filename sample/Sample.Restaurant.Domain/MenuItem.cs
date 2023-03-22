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

    public static MenuItem[] DefaultData => GenerateDefaultData();

    // Not really nullable but made nullable to prevent EF creating new enum table record
    public EnumEntity<MenuItemType>? ItemType { get; protected set; }
    public MenuItemType ItemTypeId { get; protected set; }

    public string Name { get; protected set; }
    public decimal Price { get; protected set; }

    internal static MenuItem EmptyMenuItem => new();

    private static MenuItem[] GenerateDefaultData()
    {
        var ids = new Guid[]
        {
            new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"),
            new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"),
            new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"),
            new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"),
            new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"),
            new Guid("58b91a73-682d-4696-b545-b493b56a0335"),
            new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5")
        };

        return new MenuItem[]
        {
            new MenuItem(ids[0], "Pizza", 15, MenuItemType.Food),
            new MenuItem(ids[1], "Pasta", 15, MenuItemType.Food),
            new MenuItem(ids[2], "Garlic Bread", 4.50M, MenuItemType.Food),
            new MenuItem(ids[3], "Ice Cream", 5, MenuItemType.Food),
            new MenuItem(ids[4], "Beer", 7.50M, MenuItemType.Drink),
            new MenuItem(ids[5], "Wine", 10, MenuItemType.Drink),
            new MenuItem(ids[6], "Soft Drink", 3.50M, MenuItemType.Drink)
        };
    }
}
