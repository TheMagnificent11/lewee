using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Orders.Application.GetOrders;

namespace Sample.BlazorServer.App.Pages;

public partial class Index
{
    private OrderDto[]? orders;

    [Inject]
    private IMediator? Mediator { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (this.Mediator == null)
        {
            this.orders = Array.Empty<OrderDto>();
            return;
        }

        var result = await this.Mediator.Send(new GetOrdersQuery(Guid.NewGuid()));

        this.orders = result.ToArray();
    }
}