namespace Sample.Restaurant.Application;

public class TableDetailsDto : TableDto
{
    public List<OrderItemDto> Items { get; set; } = new();

    public static TableDetailsDto Create(TableDto table, List<MenuItemDto> menuItems)
    {
        var details = new TableDetailsDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            IsInUse = table.IsInUse
        };

        menuItems.ForEach(x => details.Items.Add(new OrderItemDto { MenuItem = x }));

        return details;
    }
}
