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

    private IMessageReceivedAction? MapOrderMessage(OrderDto order, Guid correlationId)
    {
        var state = this.orderState.Value;

        if (state.CorrelationId != correlationId || state.PendingCommandType is null)
        {
            this.logger.LogWarning(
                "No pending order command found for correlation ID {CorrelationId}",
                correlationId);

            return null;
        }

        return state.PendingCommandType switch
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
    }
}
