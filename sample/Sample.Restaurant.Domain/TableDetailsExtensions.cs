namespace Sample.Restaurant.Domain;

public static class TableDetailsExtensions
{
    public static TableDetails AddOrderItem(this TableDetails tableDetails, OrderItemAddedDomainEvent domainEvent)
    {
        return UpdatedOrderItems(tableDetails, domainEvent.MenuItemId, decreaseQuantity: false, domainEvent.CorrelationId);
    }

    public static TableDetails RemoveOrderItem(this TableDetails tableDetails, OrderItemRemovedDomainEvent domainEvent)
    {
        return UpdatedOrderItems(tableDetails, domainEvent.MenuItemId, decreaseQuantity: true, domainEvent.CorrelationId);
    }

    private static TableDetails UpdatedOrderItems(
        TableDetails tableDetails,
        Guid menuItemId,
        bool decreaseQuantity,
        Guid correlationId)
    {
        var orderItem = tableDetails.Items.FirstOrDefault(x => x.MenuItem.Id == menuItemId);
        if (orderItem == null)
        {
            return tableDetails;
        }

        var newQuantity = decreaseQuantity
            ? orderItem.Quantity > 1 ? orderItem.Quantity - 1 : 0
            : orderItem.Quantity++;

        var updatedOrderItem = new OrderItemDetails
        {
            MenuItem = orderItem.MenuItem,
            Quantity = newQuantity
        };

        var newOrderItems = tableDetails.Items
            .Where(x => x.MenuItem.Id != menuItemId)
            .ToList();

        newOrderItems.Add(updatedOrderItem);

        return tableDetails with { CorrelationId = correlationId, Items = newOrderItems };
    }
}
