using Sample.Restaurant.Client.States.Tables.Actions;
using Sample.Restaurant.Client.States.UseTable.Actions;

namespace Sample.Restaurant.Client.Pages;

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
