using Lewee.Blazor.Fluxor.Actions;
using Lewee.Blazor.Messages;
using Sample.Restaurant.App.States.UseTable.Actions;
using Sample.Restaurant.Contracts.ClientMessages;

namespace Sample.Restaurant.App.States;

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

        return null;
    }
}
