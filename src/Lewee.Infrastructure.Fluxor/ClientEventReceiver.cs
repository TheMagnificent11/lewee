using System.Text.Json;
using Fluxor;
using Lewee.Application.ServerSentEvents;
using Lewee.Common;
using Lewee.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Client Event Receiver Component
/// </summary>
/// <remarks>
/// Subscribes to client events from the broadcaster and dispatches corresponding Fluxor actions.
/// This component should be placed in the application layout to receive events for the authenticated user.
/// </remarks>
public sealed class ClientEventReceiver : ComponentBase, IDisposable
{
    private string? currentUserId;

    /// <summary>
    /// Gets or sets the client event broadcaster
    /// </summary>
    [Inject]
    public IClientEventBroadcaster ClientEventBroadcaster { get; set; } = null!;

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
    public void Dispose()
    {
        this.ClientEventBroadcaster.OnClientEvent -= this.HandleClientEvent;
        this.Logger.LogInformation("Stopped listening for client events");
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        this.currentUserId = this.AuthenticatedUserService.UserId;
        this.ClientEventBroadcaster.OnClientEvent += this.HandleClientEvent;

        this.Logger.LogInformation("Started listening for client events. UserId: {UserId}", this.currentUserId);
    }

    private void HandleClientEvent(object? sender, ClientEventArgs e)
    {
        var clientEvent = e.ClientEvent;

        // Filter by user ID - only process events for this user
        if (clientEvent.UserId != null && clientEvent.UserId != this.currentUserId)
        {
            return;
        }

        using (this.Logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [LoggingConsts.CorrelationId] = clientEvent.CorrelationId,
            ["ContractType"] = clientEvent.ContractFullClassName,
        }))
        {
            this.Logger.LogInformation("Processing client event");

            // Deserialize the message
            var messageType = Type.GetType($"{clientEvent.ContractFullClassName}, {clientEvent.ContractAssemblyName}");
            if (messageType == null)
            {
                this.Logger.LogWarning("Could not resolve type: {TypeName}", clientEvent.ContractFullClassName);
                return;
            }

            var message = JsonSerializer.Deserialize(clientEvent.MessageJson, messageType);
            if (message == null)
            {
                this.Logger.LogWarning("Could not deserialize message");
                return;
            }

            // Map the message to an action
            var action = this.MessageMapper.Map(message, clientEvent.CorrelationId);
            if (action == null)
            {
                this.Logger.LogDebug("No action mapped for message type: {MessageType}", messageType.FullName);
                return;
            }

            // Dispatch the action - use InvokeAsync to ensure thread safety with Blazor
            _ = this.InvokeAsync(async () =>
            {
                try
                {
                    this.Dispatcher.Dispatch(action);
                    this.Logger.LogInformation("Dispatched action: {ActionType}", action.GetType().FullName);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Error dispatching action: {ActionType}", action.GetType().FullName);
                }

                await Task.CompletedTask;
            });
        }
    }
}
