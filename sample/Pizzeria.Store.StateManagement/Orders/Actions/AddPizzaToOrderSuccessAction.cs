using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderSuccessAction(Guid CorrelationId) : IRequestSuccessAction;
