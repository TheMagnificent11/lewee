using Fluxor;
using Lewee.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Messaging;

/// <summary>
/// Message Receiver Initializer
/// </summary>
public class MessageReceiverInitializer : ComponentBase
{
    [Inject]
    private HubConnection HubConnection { get; set; }

    [Inject]
    private MessageDeserializer MessageDeserializer { get; set; }

    [Inject]
    private IMessageToActionMapper MessageToActionMapper { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; set; }

    [Inject]
    private ILogger<MessageReceiverInitializer> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await this.HubConnection.StartAsync();

        this.HubConnection.On<ClientMessage>("ReceiveMessage", message =>
        {
            var (messageBody, correlationId) = this.MessageDeserializer.Deserialize(message);
            if (messageBody == null)
            {
                return;
            }

            var action = this.MessageToActionMapper.Map(messageBody, correlationId ?? Guid.Empty);
            if (action == null)
            {
                this.Logger.LogInformation("No action mapped to {@MessageBody}", messageBody);
                return;
            }

            this.Dispatcher.Dispatch(action);
            this.Logger.LogInformation(
                "Action Type {Action} dispatched (Message Body: {@MessageBody})",
                action.GetType().Name,
                messageBody);
        });
    }
}
