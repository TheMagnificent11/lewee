using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Tests.Integration;

public class MessageToActionMapper : IMessageToActionMapper
{
    private readonly ILogger<MessageToActionMapper> logger;

    public MessageToActionMapper(ILogger<MessageToActionMapper> logger)
    {
        this.logger = logger;
    }

    public IMessageReceivedAction Map(object message, Guid correlationId)
    {
        this.logger.LogInformation(
            "SignalR message received: {MessageType}, CorrelationId: {CorrelationId}",
            message.GetType().Name,
            correlationId);
        
        return message switch
        {
            PizzaOrder order => new OrderCreatedAction(order, correlationId),
            _ => null
        };
    }
}

public record OrderCreatedAction(PizzaOrder Order, Guid CorrelationId) : IMessageReceivedAction;
