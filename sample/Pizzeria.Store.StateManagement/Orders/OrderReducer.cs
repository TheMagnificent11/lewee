using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public static class OrderReducer
{
    [ReducerMethod]
    public static OrderState OnStartOrder(
        [NotNull] OrderState state,
        [NotNull] StartOrderAction action)
    {
        return state.OnCommand<OrderState, OrderDto, StartOrderAction>(action, clearData: true);
    }

    [ReducerMethod]
    public static OrderState OnStartOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] StartOrderSuccessAction action)
    {
        return state.OnCommandSuccess<OrderState, OrderDto, StartOrderSuccessAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnStartOrderFailure(
        [NotNull] OrderState state,
        [NotNull] StartOrderFailureAction action)
    {
        return state.OnCommandError<OrderState, OrderDto, StartOrderFailureAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnStartOrderCompleted(
        [NotNull] OrderState state,
        [NotNull] StartOrderCompletedAction action)
    {
        return state.OnCommandCompleted<OrderState, OrderDto, StartOrderCompletedAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrder(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderAction action)
    {
        return state.OnCommand<OrderState, OrderDto, AddPizzaToOrderAction>(action, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderSuccessAction action)
    {
        return state.OnCommandSuccess<OrderState, OrderDto, AddPizzaToOrderSuccessAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrderFailure(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderFailureAction action)
    {
        return state.OnCommandError<OrderState, OrderDto, AddPizzaToOrderFailureAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnClearOrderError(
        [NotNull] OrderState state,
        ClearOrderErrorAction _)
    {
        return state with
        {
            ErrorMessage = null,
        };
    }
}
