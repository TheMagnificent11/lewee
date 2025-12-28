using System.Diagnostics.CodeAnalysis;
using Lewee.Blazor.Messaging;
using Lewee.Infrastructure.Fluxor;
using Lewee.Tests.Contracts;

namespace Lewee.Blazor.Tests.Web;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via DI")]
internal sealed class MessageToActionMapper : IMessageToActionMapper
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
            PizzaOrder order => new OrderCreatedAction { CorrelationId = correlationId, Data = order },
            _ => null,
        };
    }
}
