using System.Diagnostics.CodeAnalysis;
using Fluxor;

namespace Pizzeria.Store.StateManagement.Orders;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via Fluxor")]
public class OrderStateFeature : Feature<OrderState>
{
    public override string GetName() => nameof(OrderState);

    protected override OrderState GetInitialState() => new();
}
