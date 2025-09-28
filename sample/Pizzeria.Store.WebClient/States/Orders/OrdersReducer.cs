using Fluxor;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.States.Orders;

public static class OrdersReducer
{
    [ReducerMethod]
    public static OrdersState OnStartOrder(OrdersState state, StartOrderAction _)
    {
        return state with
        {
            IsStartingOrder = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderSuccess(OrdersState state, StartOrderSuccessAction action)
    {
        return state with
        {
            CurrentOrderId = action.OrderId,
            IsStartingOrder = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderFailure(OrdersState state, StartOrderFailureAction action)
    {
        return state with
        {
            IsStartingOrder = false,
            ErrorMessage = action.ErrorMessage
        };
    }

    [ReducerMethod]
    public static OrdersState OnAddPizzaToOrderSuccess(OrdersState state, AddPizzaToOrderSuccessAction action)
    {
        var newQuantities = new Dictionary<Guid, int>(state.PizzaQuantities);
        newQuantities[action.PizzaId] = newQuantities.GetValueOrDefault(action.PizzaId, 0) + 1;

        return state with
        {
            PizzaQuantities = newQuantities,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static OrdersState OnAddPizzaToOrderFailure(OrdersState state, AddPizzaToOrderFailureAction action)
    {
        return state with
        {
            ErrorMessage = action.ErrorMessage
        };
    }

    [ReducerMethod]
    public static OrdersState OnClearOrderError(OrdersState state, ClearOrderErrorAction _)
    {
        return state with
        {
            ErrorMessage = null
        };
    }
}