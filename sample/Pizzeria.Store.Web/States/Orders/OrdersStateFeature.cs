using Fluxor;

namespace Pizzeria.Store.Web.States.Orders;

internal sealed class OrdersStateFeature : Feature<OrdersState>
{
    public override string GetName() => nameof(OrdersState);

    protected override OrdersState GetInitialState() => new();
}
