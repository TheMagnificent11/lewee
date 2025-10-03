using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record StartOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
