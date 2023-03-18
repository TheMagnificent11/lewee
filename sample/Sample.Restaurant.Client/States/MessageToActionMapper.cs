using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messaging;
using Sample.Restaurant.Client.States.TableDetails.Actions;
using Sample.Restaurant.Client.States.UseTable.Actions;
using Sample.Restaurant.Contracts.ClientMessages;

namespace Sample.Restaurant.Client.States;

public class MessageToActionMapper : IMessageToActionMapper
{
    public IMessageReceivedAction? Map(object message, Guid correlationId)
    {
        var type = message.GetType();

        if (type == typeof(TableUsedMessage))
        {
            var tableUsedMessage = (TableUsedMessage)message;
            return new UseTableCompletedAction(correlationId, tableUsedMessage.TableNumber);
        }

        if (type == typeof(ItemOrderedMessage))
        {
            var itemOrderedMessage = (ItemOrderedMessage)message;
            return new OrderItemCompletedAction(correlationId, itemOrderedMessage.TableNumber);
        }

        return null;
    }
}
