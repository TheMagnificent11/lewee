using Microsoft.AspNetCore.Components;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client.Pages;

public partial class Home
{
    [Inject]
    private IOrdersApi OrdersApi { get; set; }

    private OrderDto[]? Orders { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.Orders = await this.OrdersApi.GetUserOrders(CancellationToken.None);
    }
}
