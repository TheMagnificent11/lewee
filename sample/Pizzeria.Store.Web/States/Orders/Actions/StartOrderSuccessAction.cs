using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.Web.States.Orders.Actions;

internal sealed record StartOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
