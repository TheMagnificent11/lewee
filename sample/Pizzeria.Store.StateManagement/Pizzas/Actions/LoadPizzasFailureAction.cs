using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Store.StateManagement.Pizzas.Actions;

public record LoadPizzasFailureAction : IRequestErrorAction
{
    public Guid CorrelationId { get; init; }

    public string ErrorMessage { get; init; }
}
