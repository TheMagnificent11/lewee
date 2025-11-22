using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.Web.States.Orders.Actions;

namespace Pizzeria.Store.Web.States.Orders;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Fluxor requires reducer methods to be public")]
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
            CurrentOrder = action.Order,
            CorrelationId = action.CorrelationId
        };
    }

    [ReducerMethod]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public static OrdersState OnAddPizzaToOrderSuccess(OrdersState state, AddPizzaToOrderSuccessAction _)
    {
        return state with
        {
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
