namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record StartOrderAction;

public record StartOrderSuccessAction(Guid OrderId);

public record StartOrderFailureAction(string ErrorMessage);

public record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);

public record AddPizzaToOrderSuccessAction(Guid PizzaId);

public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);

public record ClearOrderErrorAction;