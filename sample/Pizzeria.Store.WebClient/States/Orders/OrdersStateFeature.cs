using Fluxor;

namespace Pizzeria.Store.WebClient.States.Orders;

public sealed class OrdersStateFeature : Feature<OrdersState>
{
    public override string GetName() => nameof(OrdersState);

    protected override OrdersState GetInitialState() => new();
}