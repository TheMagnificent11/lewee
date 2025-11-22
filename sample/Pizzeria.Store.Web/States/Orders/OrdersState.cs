using System.Diagnostics.CodeAnalysis;
using Lewee.Blazor.Fluxor;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Orders;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Fluxor requires state to be public")]
public record OrdersState : RequestState
{
    public OrderDto? CurrentOrder { get; init; }
    public bool IsStartingOrder { get; init; } = false;
}
