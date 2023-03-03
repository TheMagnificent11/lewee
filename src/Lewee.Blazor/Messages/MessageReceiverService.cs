using Fluxor;
using Lewee.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Messages;

internal class MessageReceiverService
{
    private readonly MessageReceiver messageReceiver;
    private readonly MessageDeserializer messageDeserializer;
    private readonly IMessageToActionMapper messageToActionMapper;
    private readonly IDispatcher dispatcher;
    private readonly ILogger<MessageReceiverService> logger;

    public MessageReceiverService(
        MessageReceiver messageReceiverService,
        MessageDeserializer messageDeserializer,
        IMessageToActionMapper messageToActionMapper,
        IDispatcher dispatcher,
        ILogger<MessageReceiverService> logger)
    {
        this.messageReceiver = messageReceiverService;
        this.messageDeserializer = messageDeserializer;
        this.messageToActionMapper = messageToActionMapper;
        this.dispatcher = dispatcher;
        this.logger = logger;
    }

    public async Task StartAsync()
    {
        await this.messageReceiver.StartAsync();

        var connection = this.messageReceiver.GetHubConnection();

        connection.On<string>(nameof(ClientMessage), message =>
        {
            var (messageBody, correlationId) = this.messageDeserializer.Deserialize(message);
            if (messageBody == null)
            {
                return;
            }

            var action = this.messageToActionMapper.Map(messageBody, correlationId ?? Guid.Empty);
            if (action == null)
            {
                this.logger.LogInformation("No action mapped to {@MessageBody}", messageBody);
                return;
            }

            this.dispatcher.Dispatch(action);
            this.logger.LogInformation(
                "Action Type {Action} dispatched (Message Body: {@MessageBody})",
                action.GetType().Name,
                messageBody);
        });
    }
}
