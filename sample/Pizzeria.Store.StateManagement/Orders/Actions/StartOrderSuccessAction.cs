using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderSuccessAction(Guid CorrelationId) : IRequestSuccessAction;
