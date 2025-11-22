using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.Web.States.Orders.Actions;

internal record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
