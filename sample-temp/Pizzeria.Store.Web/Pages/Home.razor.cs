using Pizzeria.Store.Web.States.Orders.Actions;

namespace Pizzeria.Store.Web.Pages;

public partial class Home
{
    private void StartNewOrder()
    {
        this.Dispatcher.Dispatch(new StartOrderAction());
    }

    private void ClearError()
    {
        this.Dispatcher.Dispatch(new ClearOrderErrorAction());
    }
}
