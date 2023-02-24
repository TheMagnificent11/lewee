using Microsoft.AspNetCore.Components;
using Sample.Restaurant.App.States.TableDetails.Actions;

namespace Sample.Restaurant.App.Pages;
public partial class Table
{
    private TableDetailsDto? table;

    [Parameter]
    public int TableNumber { get; set; }

    protected override void OnInitialized()
    {
        this.Dispatcher.Dispatch(new GetTableDetailsAction(Guid.NewGuid(), this.TableNumber));
    }

    private void AddToOrder(Guid menuItemId)
    {
        // this.Dispatcher.Dispatch(new UseTableAction(Guid.NewGuid(), this.TableNumber));
    }
}
