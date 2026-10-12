using Pizzeria.Ordering.StateManagement.Orders.Actions;

namespace Pizzeria.Ordering.Components;

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

    public static class Selectors
    {
        public const string StartOrderButton = $"[role='button'][aria-label='{AriaLabels.StartOrder}']";
    }

    private static class AriaLabels
    {
        public const string StartOrder = "start order";
    }
}
