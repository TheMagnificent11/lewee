using System.Diagnostics.CodeAnalysis;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.Web.Infrastructure;

public class MessageToActionMapper //: IMessageToActionMapper
{
    private readonly ILogger<MessageToActionMapper> logger;

    public MessageToActionMapper(ILogger<MessageToActionMapper> logger)
    {
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
                OrderDto order => new StartOrderCompletedAction
                {
                    Data = order,
                    CorrelationId = correlationId,
                },
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
}
