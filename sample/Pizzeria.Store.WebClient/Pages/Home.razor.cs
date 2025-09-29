using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.WebClient.States.Orders;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.Pages;

public partial class Home : FluxorComponent
{
    [Inject] private IState<OrdersState> OrdersState { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private void StartNewOrder()
    {
        Dispatcher.Dispatch(new StartOrderAction());
    }

    private void ClearError()
    {
        Dispatcher.Dispatch(new ClearOrderErrorAction());
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && OrdersState.Value.CurrentOrderId != null)
        {
            Navigation.NavigateTo("/order");
        }
    }
}