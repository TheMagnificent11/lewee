using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderFailureAction(Guid CorrelationId, string ErrorMessage)
    : IRequestErrorAction;
