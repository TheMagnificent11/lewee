using System.Diagnostics.CodeAnalysis;
using Lewee.Blazor.Fluxor.Actions;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Orders.Actions;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Fluxor actions must be public")]
public record AddPizzaToOrderCompletedAction : IMessageReceivedAction
{
    public AddPizzaToOrderCompletedAction(OrderDto order, Guid correlationId)
    {
        this.Order = order;
        this.CorrelationId = correlationId;
    }

    public OrderDto Order { get; init; }
    public Guid CorrelationId { get; init; }
}
