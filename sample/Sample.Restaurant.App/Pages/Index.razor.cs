using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.App.States.UseTable.Actions;

namespace Sample.Restaurant.App.Pages;

public partial class Index
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.Dispatcher.Dispatch(new GetTablesAction(Guid.NewGuid()));
    }

    private void UseTable(int tableNumber)
    {
        this.Dispatcher.Dispatch(new UseTableAction(Guid.NewGuid(), tableNumber));
    }

    private void ViewTable(int tableNumber)
    {
        this.NavigationManager.NavigateTo($"tables/{tableNumber}");
    }
}
