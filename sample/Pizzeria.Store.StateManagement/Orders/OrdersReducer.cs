using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public static class OrdersReducer
{
    [ReducerMethod]
    public static OrdersState OnStartOrder(
        [NotNull] OrdersState state,
        [NotNull] StartOrderAction action)
    {
        return state with
        {
            IsStartingOrder = true,
            ErrorMessage = null,
            CorrelationId = action.CorrelationId,
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderSuccess(
        [NotNull] OrdersState state,
        [NotNull] StartOrderSuccessAction action)
    {
        return state with
        {
            IsStartingOrder = false,
            ErrorMessage = null,
            CorrelationId = action.CorrelationId,
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderFailure(
        [NotNull] OrdersState state,
        [NotNull] StartOrderFailureAction action)
    {
        return state with
        {
            IsStartingOrder = false,
            ErrorMessage = action.ErrorMessage,
            CorrelationId = action.CorrelationId,
        };
    }

    [ReducerMethod]
    public static OrdersState OnStartOrderCompleted(
        [NotNull] OrdersState state,
        [NotNull] StartOrderCompletedAction action)
    {
        return state with
        {
            CurrentOrder = action.Order,
            CorrelationId = action.CorrelationId,
        };
    }

    [ReducerMethod]
    public static OrdersState OnAddPizzaToOrderSuccess(
        [NotNull] OrdersState state,
        [NotNull] AddPizzaToOrderSuccessAction _)
    {
        return state with
        {
            ErrorMessage = null,
        };
    }

    [ReducerMethod]
    public static OrdersState OnAddPizzaToOrderFailure(
        [NotNull] OrdersState state,
        [NotNull] AddPizzaToOrderFailureAction action)
    {
        return state with
        {
            ErrorMessage = action.ErrorMessage,
        };
    }

    [ReducerMethod]
    public static OrdersState OnClearOrderError(
        [NotNull] OrdersState state,
        ClearOrderErrorAction _)
    {
        return state with
        {
            ErrorMessage = null,
        };
    }
}
