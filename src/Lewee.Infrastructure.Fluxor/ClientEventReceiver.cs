using System.Text.Json;
using Fluxor;
using Lewee.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Client Event Receiver Component
/// </summary>
/// <remarks>
/// Subscribes to client events via SSE and dispatches corresponding Fluxor actions.
/// This component should be placed in the application layout to receive events for the authenticated user.
/// SSE connection is only established after interactive rendering (not during pre-rendering).
/// </remarks>
public sealed class ClientEventReceiver : ComponentBase, IAsyncDisposable
{
    private bool isListening;

    /// <summary>
    /// Gets or sets the SSE client message receiver
    /// </summary>
    [Inject]
    public SseClientMessageReceiver MessageReceiver { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Fluxor dispatcher
    /// </summary>
    [Inject]
    public IDispatcher Dispatcher { get; set; } = null!;

    /// <summary>
    /// Gets or sets the message to action mapper
    /// </summary>
    [Inject]
    public IMessageToActionMapper MessageMapper { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authenticated user service
    /// </summary>
    [Inject]
    public IAuthenticatedUserService AuthenticatedUserService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger
    /// </summary>
    [Inject]
    public ILogger<ClientEventReceiver> Logger { get; set; } = null!;

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!this.isListening)
        {
            return;
        }

        this.MessageReceiver.OnMessageReceived -= this.HandleClientMessage;
        await this.MessageReceiver.DisposeAsync();
        this.Logger.LogStoppedListening();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (this.isListening)
        {
            return;
        }

        var userId = this.AuthenticatedUserService.UserId;

        // Only start listening if the user is authenticated
        if (string.IsNullOrEmpty(userId))
        {
            if (firstRender)
            {
                this.Logger.LogSkippingUnauthenticated();
            }

            return;
        }

        this.MessageReceiver.OnMessageReceived += this.HandleClientMessage;

        await this.MessageReceiver.StartAsync();
        this.isListening = true;
        this.Logger.LogStartedListening(userId);
    }

    private void HandleClientMessage(object? sender, ClientMessageEventArgs e)
    {
        var clientMessage = e.Message;

        using (this.Logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [LoggingConsts.CorrelationId] = clientMessage.CorrelationId,
            ["ContractType"] = clientMessage.ContractFullClassName,
        }))
        {
            this.Logger.LogProcessingClientEvent();

            var messageType = Type.GetType($"{clientMessage.ContractFullClassName}, {clientMessage.ContractAssemblyName}");
            if (messageType == null)
            {
                this.Logger.LogCouldNotResolveType(clientMessage.ContractFullClassName);
                return;
            }

            var message = JsonSerializer.Deserialize(clientMessage.MessageJson, messageType);
            if (message == null)
            {
                this.Logger.LogCouldNotDeserializeMessage();
                return;
            }

            var action = this.MessageMapper.Map(message, clientMessage.CorrelationId);
            if (action == null)
            {
                this.Logger.LogNoActionMapped(messageType.FullName);
                return;
            }

            _ = this.InvokeAsync(() =>
            {
                using (this.Logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [LoggingConsts.CorrelationId] = clientMessage.CorrelationId,
                    ["ContractType"] = clientMessage.ContractFullClassName,
                }))
                {
                    try
                    {
                        this.Dispatcher.Dispatch(action);
                        this.Logger.LogDispatchedAction(action.GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogErrorDispatchingAction(ex, action.GetType().FullName);
                    }
                }
            });
        }
    }
}
