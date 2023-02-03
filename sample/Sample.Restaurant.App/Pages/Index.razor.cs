using Sample.Restaurant.App.States.TableDetails.Actions;
using Sample.Restaurant.App.States.Tables.Actions;

namespace Sample.Restaurant.App.Pages;

public partial class Index
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.Dispatcher.Dispatch(new GetTables(Guid.NewGuid()));
    }

    private void UseTable(int tableNumber)
    {
        this.Dispatcher.Dispatch(new GetTableDetails(Guid.NewGuid(), tableNumber));
    }

    private void ViewTable(int tableNumber)
    {
        this.NavigationManager.NavigateTo($"tables/{tableNumber}");
    }
}
