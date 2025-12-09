using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.Web.Orders.Actions;

public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
