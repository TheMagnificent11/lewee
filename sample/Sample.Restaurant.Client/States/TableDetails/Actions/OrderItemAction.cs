using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record OrderItemAction : IRequestAction
{
    public OrderItemAction(Guid correlationId, int tableNumber, Guid menuItemId)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
        this.MenuItemId = menuItemId;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
    public Guid MenuItemId { get; }
}
