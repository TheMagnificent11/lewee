using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.Pages;
public partial class Table
{
    private TableDetailsDto? table;

    [Parameter]
    public int TableNumber { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await this.Mediator.Send(new GetTableDetailsQuery(Guid.NewGuid(), this.TableNumber));

        if (!result.IsSuccess || result.Data == null)
        {
            // TODO: error handling
            return;
        }

        this.table = result.Data;
    }

    private async Task AddToOrder(Guid menuItemId)
    {
        // TODO: error handling
        _ = await this.Mediator.Send(new AddMenuItemCommand(Guid.NewGuid(), this.TableNumber, menuItemId));
    }
}
