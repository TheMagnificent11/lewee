using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.States.Orders;

public static class OrdersReducer
{
    [ReducerMethod]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public static OrdersState OnStartOrder(OrdersState state, StartOrderAction action)
    {
        return state with
        {
            IsStartingOrder = true,
            ErrorMessage = null,
            CorrelationId = action.CorrelationId
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderSuccess(OrdersState state, StartOrderSuccessAction action)
    {
        return state with
        {
            IsStartingOrder = false,
            ErrorMessage = null,
            CorrelationId = action.CorrelationId
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderFailure(OrdersState state, StartOrderFailureAction action)
    {
        return state with
        {
            IsStartingOrder = false,
            ErrorMessage = action.ErrorMessage,
            CorrelationId = action.CorrelationId
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderCompleted(OrdersState state, StartOrderCompletedAction action)
    {
        return state with
        {
            CurrentOrderId = action.OrderId,
            CorrelationId = action.CorrelationId
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
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public static OrdersState OnClearOrderError(OrdersState state, ClearOrderErrorAction _)
    {
        return state with
        {
            ErrorMessage = null
        };
    }
}
