using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
