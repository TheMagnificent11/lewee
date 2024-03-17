using Fluxor;
using Microsoft.AspNetCore.Components;
using Sample.Pizzeria.Client.Orders.GetOrders;

namespace Sample.Pizzeria.Client.Orders;

public partial class Orders
{
    [Inject]
    private IDispatcher Dispatcher { get; set; }

    [Inject]
    private IState<GetOrdersState> State { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.Dispatcher.Dispatch(new GetOrdersAction());
    }
}
