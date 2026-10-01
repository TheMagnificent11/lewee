using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

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
    public static OrderState OnRemovePizzaFromOrder(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderAction action)
    {
        return state.OnCommand<OrderState, OrderDto, RemovePizzaFromOrderAction>(action, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnRemovePizzaFromOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderSuccessAction action)
    {
        return state.OnCommandSuccess<OrderState, OrderDto, RemovePizzaFromOrderSuccessAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnRemovePizzaFromOrderFailure(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderFailureAction action)
    {
        return state.OnCommandError<OrderState, OrderDto, RemovePizzaFromOrderFailureAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrder(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderAction action)
    {
        return state.OnCommand<OrderState, OrderDto, SubmitPickupOrderAction>(action, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderSuccessAction action)
    {
        return state.OnCommandSuccess<OrderState, OrderDto, SubmitPickupOrderSuccessAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrderFailure(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderFailureAction action)
    {
        return state.OnCommandError<OrderState, OrderDto, SubmitPickupOrderFailureAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrder(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderAction action)
    {
        return state.OnCommand<OrderState, OrderDto, SubmitDeliveryOrderAction>(action, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderSuccessAction action)
    {
        return state.OnCommandSuccess<OrderState, OrderDto, SubmitDeliveryOrderSuccessAction>(action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrderFailure(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderFailureAction action)
    {
        return state.OnCommandError<OrderState, OrderDto, SubmitDeliveryOrderFailureAction>(action);
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
