using System.Diagnostics.CodeAnalysis;
using Fluxor;

namespace Pizzeria.Store.Web.States.Orders;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Fluxor requires state feature to be public")]
public sealed class OrdersStateFeature : Feature<OrdersState>
{
    public override string GetName() => nameof(OrdersState);

    protected override OrdersState GetInitialState() => new();
}
