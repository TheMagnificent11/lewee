using System.Diagnostics.CodeAnalysis;
using Lewee.Blazor.Fluxor.Actions;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Orders.Actions;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Not used yet, but will be used")]
internal sealed record AddPizzaToOrderCompletedAction : IMessageReceivedAction
{
    public AddPizzaToOrderCompletedAction(OrderDto order, Guid correlationId)
    {
        this.Order = order;
        this.CorrelationId = correlationId;
    }

    public OrderDto Order { get; init; }
    public Guid CorrelationId { get; init; }
}
