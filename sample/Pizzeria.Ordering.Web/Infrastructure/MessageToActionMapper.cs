using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.Web.Infrastructure;

internal sealed class MessageToActionMapper : IMessageToActionMapper
{
    private readonly IState<OrderState> orderState;
    private readonly ILogger<MessageToActionMapper> logger;

    public MessageToActionMapper(
        IState<OrderState> orderState,
        ILogger<MessageToActionMapper> logger)
    {
        this.orderState = orderState;
        this.logger = logger;
    }

    public IMessageReceivedAction? Map([NotNull] object message, Guid correlationId)
    {
        var messageType = message.GetType().FullName;

        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [LoggingConsts.CorrelationId] = correlationId,
            ["MessageType"] = messageType!,
        }))
        {
            this.logger.LogInformation("Mapping message");

            var result = message switch
            {
                OrderDto order => this.MapOrderMessage(order, correlationId),
                _ => null,
            };

            if (result == null)
            {
                this.logger.LogInformation("No mapping found for message");
            }
            else
            {
                this.logger.LogInformation(
                    "Successfully mapped message to {ActionType}",
                    result.GetType().FullName);
            }

            return result;
        }
    }

    private static IMessageReceivedAction<OrderDto>? MapPendingOrderCommand(
        OrderDto order,
        Guid correlationId,
        OrderCommandType commandType) =>
        commandType switch
        {
            OrderCommandType.StartOrder => new StartOrderCompletedAction
            {
                Data = order,
                CorrelationId = correlationId,
            },
            OrderCommandType.AddPizzaToOrder => new AddPizzaToOrderCompletedAction
            {
                Data = order,
                CorrelationId = correlationId,
            },
            OrderCommandType.RemovePizzaFromOrder => new RemovePizzaFromOrderCompletedAction
            {
                Data = order,
                CorrelationId = correlationId,
            },
            OrderCommandType.SubmitPickupOrder => new SubmitPickupOrderCompletedAction
            {
                Data = order,
                CorrelationId = correlationId,
            },
            OrderCommandType.SubmitDeliveryOrder => new SubmitDeliveryOrderCompletedAction
            {
                Data = order,
                CorrelationId = correlationId,
            },
            _ => null,
        };

    private IMessageReceivedAction? MapOrderMessage(OrderDto order, Guid correlationId)
    {
        var state = this.orderState.Value;

        if (state.PendingCommands.TryGetValue(correlationId, out var commandType))
        {
            return MapPendingOrderCommand(order, correlationId, commandType);
        }

        if (state.Data?.Id == order.Id)
        {
            return new OrderUpdatedAction { Data = order, CorrelationId = correlationId };
        }

        this.logger.LogWarning(
            "No pending order command or current order found for correlation ID {CorrelationId}",
            correlationId);

        return null;
    }
}
