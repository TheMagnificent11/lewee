using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
