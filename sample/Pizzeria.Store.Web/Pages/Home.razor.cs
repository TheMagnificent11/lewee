using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.Web.States.Orders;
using Pizzeria.Store.Web.States.Orders.Actions;

namespace Pizzeria.Store.Web.Pages;

public partial class Home : FluxorComponent
{
    [Inject]
    private IState<OrdersState> OrdersState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void StartNewOrder()
    {
        this.Dispatcher.Dispatch(new StartOrderAction());
    }

    private void ClearError()
    {
        this.Dispatcher.Dispatch(new ClearOrderErrorAction());
    }
}
