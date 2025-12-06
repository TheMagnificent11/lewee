using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Web.Orders.Actions;

namespace Pizzeria.Store.Web.Infrastructure;

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
            this.logger.LogReceivedNullMessage(correlationId);
            return null;
        }

        this.logger.LogMappingMessage(
            message.GetType().FullName,
            correlationId);

        var result = message switch
        {
            OrderDto order => new StartOrderCompletedAction(order, correlationId),
            _ => null,
        };

        if (result != null)
        {
            this.logger.LogSuccessfullyMapped(
                message.GetType().Name,
                result.GetType().Name);
        }
        else
        {
            this.logger.LogNoMappingFound(message.GetType().FullName);
        }

        return result;
    }
}
