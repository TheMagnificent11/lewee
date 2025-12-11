using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderFailureAction(Guid CorrelationId, string ErrorMessage)
    : IRequestErrorAction;
