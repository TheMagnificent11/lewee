using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.Web.Orders.Actions;

public record StartOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
