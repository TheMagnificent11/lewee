using Fluxor;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Events;

internal class MessageReceiverService : IHostedService
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.messageReceiver.StartAsync();

        var connection = this.messageReceiver.GetHubConnection();

        connection.On<string>("ReceiveMessage", message =>
        {
            var messageBody = this.messageDeserializer.Deserialize(message);
            if (messageBody == null)
            {
                return;
            }

            var action = this.messageToActionMapper.Map(messageBody);
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

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await this.messageReceiver.StopAsync();
    }
}
