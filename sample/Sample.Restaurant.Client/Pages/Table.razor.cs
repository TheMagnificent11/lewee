using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Client.States.TableDetails.Actions;

namespace Sample.Restaurant.Client.Pages;

public partial class Table
{
    [Parameter]
    public int TableNumber { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.Dispatcher.Dispatch(new GetTableDetailsAction(Guid.NewGuid(), this.TableNumber));
    }

    private void AddToOrder(Guid menuItemId)
    {
        throw new NotImplementedException();
    }
}
