using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.Pages;

public partial class Index
{
    protected override void OnInitialized()
    {
        this.Dispatcher.Dispatch(new GetTables(Guid.NewGuid()));
    }

    private async Task UseTable(int tableNumber)
    {
        var result = await this.Mediator.Send(new UseTableCommand(Guid.NewGuid(), tableNumber));

        if (!result.IsSuccess)
        {
            // TODO: error handling
        }

        this.ViewTable(tableNumber);
    }

    private void ViewTable(int tableNumber)
    {
        this.NavigationManager.NavigateTo($"tables/{tableNumber}");
    }
}
