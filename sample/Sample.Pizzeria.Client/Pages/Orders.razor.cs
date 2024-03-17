using Fluxor;
using Microsoft.AspNetCore.Components;
using Sample.Pizzeria.Client.State.GetOrders;

namespace Sample.Pizzeria.Client.Pages;

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
