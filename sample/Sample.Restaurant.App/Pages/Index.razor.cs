using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Application.Tables;

namespace Sample.Restaurant.App.Pages;

public partial class Index
{
    private TableDto[]? tables;

    [Inject]
    private IMediator? Mediator { get; set; }

    [Inject]
    private NavigationManager? Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (this.Mediator == null)
        {
            this.tables = Array.Empty<TableDto>();
            return;
        }

        var result = await this.Mediator.Send(new GetTablesQuery(Guid.NewGuid()));

        this.tables = result.ToArray();
    }
}