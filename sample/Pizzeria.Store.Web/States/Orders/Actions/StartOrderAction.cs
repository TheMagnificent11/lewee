using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.Web.States.Orders.Actions;

public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
