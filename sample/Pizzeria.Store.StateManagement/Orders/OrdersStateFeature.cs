using System.Diagnostics.CodeAnalysis;
using Fluxor;

namespace Pizzeria.Store.StateManagement.Orders;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via Fluxor")]
public class OrdersStateFeature : Feature<OrdersState>
{
    public override string GetName() => nameof(OrdersState);

    protected override OrdersState GetInitialState() => new();
}
