using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.States;

public class MessageToActionMapper : IMessageToActionMapper
{
    private readonly ILogger<MessageToActionMapper> logger;

    public MessageToActionMapper(ILogger<MessageToActionMapper> logger)
    {
        this.logger = logger;
    }

    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        if (message == null)
        {
            this.logger.LogWarning("Received null message with CorrelationId={CorrelationId}", correlationId);
            return null;
        }

        this.logger.LogInformation(
            "Mapping message: Type={MessageType}, CorrelationId={CorrelationId}, Message={@Message}",
            message.GetType().FullName,
            correlationId,
            message);

        var result = message switch
        {
            OrderDto order => new StartOrderCompletedAction(order, correlationId),
            _ => null
        };

        if (result != null)
        {
            this.logger.LogInformation(
                "Successfully mapped {MessageType} to {ActionType}",
                message.GetType().Name,
                result.GetType().Name);
        }
        else
        {
            this.logger.LogWarning("No mapping found for message type {MessageType}", message.GetType().FullName);
        }

        return result;
    }
}
