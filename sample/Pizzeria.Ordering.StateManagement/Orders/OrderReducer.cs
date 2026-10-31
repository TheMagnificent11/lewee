using System.Collections.Immutable;
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
        return OnCommandStarted(state, action, OrderCommandType.StartOrder, clearData: true);
    }

    [ReducerMethod]
    public static OrderState OnStartOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] StartOrderSuccessAction action)
    {
        return OnCommandSucceeded(state, action);
    }

    [ReducerMethod]
    public static OrderState OnStartOrderFailure(
        [NotNull] OrderState state,
        [NotNull] StartOrderFailureAction action)
    {
        return OnCommandFailed(state, action);
    }

    [ReducerMethod]
    public static OrderState OnStartOrderCompleted(
        [NotNull] OrderState state,
        [NotNull] StartOrderCompletedAction action)
    {
        return OnCommandCompleted(state, action);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrder(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderAction action)
    {
        return OnCommandStarted(state, action, OrderCommandType.AddPizzaToOrder, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderSuccessAction action)
    {
        return OnCommandSucceeded(state, action);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrderFailure(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderFailureAction action)
    {
        return OnCommandFailed(state, action);
    }

    [ReducerMethod]
    public static OrderState OnAddPizzaToOrderCompleted(
        [NotNull] OrderState state,
        [NotNull] AddPizzaToOrderCompletedAction action)
    {
        return OnCommandCompleted(state, action);
    }

    [ReducerMethod]
    public static OrderState OnRemovePizzaFromOrder(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderAction action)
    {
        return OnCommandStarted(state, action, OrderCommandType.RemovePizzaFromOrder, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnRemovePizzaFromOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderSuccessAction action)
    {
        return OnCommandSucceeded(state, action);
    }

    [ReducerMethod]
    public static OrderState OnRemovePizzaFromOrderFailure(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderFailureAction action)
    {
        return OnCommandFailed(state, action);
    }

    [ReducerMethod]
    public static OrderState OnRemovePizzaFromOrderCompleted(
        [NotNull] OrderState state,
        [NotNull] RemovePizzaFromOrderCompletedAction action)
    {
        return OnCommandCompleted(state, action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrder(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderAction action)
    {
        return OnCommandStarted(state, action, OrderCommandType.SubmitPickupOrder, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderSuccessAction action)
    {
        return OnCommandSucceeded(state, action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrderFailure(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderFailureAction action)
    {
        return OnCommandFailed(state, action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitPickupOrderCompleted(
        [NotNull] OrderState state,
        [NotNull] SubmitPickupOrderCompletedAction action)
    {
        return OnCommandCompleted(state, action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrder(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderAction action)
    {
        return OnCommandStarted(state, action, OrderCommandType.SubmitDeliveryOrder, clearData: false);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrderSuccess(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderSuccessAction action)
    {
        return OnCommandSucceeded(state, action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrderFailure(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderFailureAction action)
    {
        return OnCommandFailed(state, action);
    }

    [ReducerMethod]
    public static OrderState OnSubmitDeliveryOrderCompleted(
        [NotNull] OrderState state,
        [NotNull] SubmitDeliveryOrderCompletedAction action)
    {
        return OnCommandCompleted(state, action);
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

    [ReducerMethod]
    public static OrderState OnOrderUpdated(
        [NotNull] OrderState state,
        [NotNull] OrderUpdatedAction action)
    {
        return state.Data is { } currentOrder
            && currentOrder.Id == action.Data.Id
            && action.Data.Revision >= currentOrder.Revision
            ? state with { Data = action.Data, ReconciliationErrorMessage = null }
            : state;
    }

    [ReducerMethod]
    public static OrderState OnOrderCommandReconciliationFailed(
        [NotNull] OrderState state,
        [NotNull] OrderCommandReconciliationFailedAction action)
    {
        var inFlightCommands = state.InFlightCommands.Remove(action.CorrelationId);
        return state with
        {
            CorrelationId = state.CorrelationId,
            ReconciliationErrorMessage = state.CorrelationId == action.CorrelationId
                ? action.ErrorMessage
                : state.ReconciliationErrorMessage,
            PendingCommands = state.PendingCommands.Remove(action.CorrelationId),
            InFlightCommands = inFlightCommands,
            IsSaving = inFlightCommands.Count > 0,
        };
    }

    [ReducerMethod]
    public static OrderState OnOrderCommandReconciled(
        [NotNull] OrderState state,
        [NotNull] OrderCommandReconciledAction action)
    {
        if (state.Data?.Id != action.Data.Id)
        {
            return state;
        }

        var currentOrder = state.Data is { } current
            && current.Id == action.Data.Id
            && current.Revision > action.Data.Revision
                ? current
                : action.Data;

        return state.OnCommandCompleted<OrderState, OrderDto, OrderCommandReconciledAction>(action) with
        {
            Data = currentOrder,
            CorrelationId = state.CorrelationId,
            PendingCommands = state.PendingCommands.Remove(action.CorrelationId),
        };
    }

    private static OrderState OnCommandStarted<TAction>(
        OrderState state,
        TAction action,
        OrderCommandType commandType,
        bool clearData)
        where TAction : IRequestAction
    {
        var stateWithCommand = state.OnCommand<OrderState, OrderDto, TAction>(action, clearData);
        var pendingCommands = clearData
            ? ImmutableDictionary<Guid, OrderCommandType>.Empty
            : state.PendingCommands;
        var inFlightCommands = clearData
            ? ImmutableHashSet<Guid>.Empty
            : state.InFlightCommands;

        return stateWithCommand with
        {
            PendingCommands = pendingCommands.SetItem(action.CorrelationId, commandType),
            InFlightCommands = inFlightCommands.Add(action.CorrelationId),
            ReconciliationErrorMessage = null,
        };
    }

    private static OrderState OnCommandSucceeded<TAction>(OrderState state, TAction action)
        where TAction : IRequestSuccessAction
    {
        var inFlightCommands = state.InFlightCommands.Remove(action.CorrelationId);
        return state.OnCommandSuccess<OrderState, OrderDto, TAction>(action) with
        {
            CorrelationId = state.CorrelationId,
            ErrorMessage = state.CorrelationId == action.CorrelationId ? null : state.ErrorMessage,
            InFlightCommands = inFlightCommands,
            IsSaving = inFlightCommands.Count > 0,
        };
    }

    private static OrderState OnCommandFailed<TAction>(OrderState state, TAction action)
        where TAction : IRequestErrorAction
    {
        var inFlightCommands = state.InFlightCommands.Remove(action.CorrelationId);
        return state.OnCommandError<OrderState, OrderDto, TAction>(action) with
        {
            CorrelationId = state.CorrelationId,
            ErrorMessage = state.CorrelationId == action.CorrelationId ? action.ErrorMessage : state.ErrorMessage,
            PendingCommands = state.PendingCommands.Remove(action.CorrelationId),
            InFlightCommands = inFlightCommands,
            IsSaving = inFlightCommands.Count > 0,
        };
    }

    private static OrderState OnCommandCompleted<TAction>(OrderState state, TAction action)
        where TAction : IMessageReceivedAction<OrderDto>
    {
        var currentOrder = state.Data is { } current
            && current.Id == action.Data.Id
            && current.Revision > action.Data.Revision
                ? current
                : action.Data;

        return state.OnCommandCompleted<OrderState, OrderDto, TAction>(action) with
        {
            Data = currentOrder,
            CorrelationId = state.CorrelationId,
            PendingCommands = state.PendingCommands.Remove(action.CorrelationId),
        };
    }
}
