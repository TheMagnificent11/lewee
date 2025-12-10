using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Lewee.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Messaging;

/// <summary>
/// Message Receiver for Blazor Server (reads from channel)
/// </summary>
public class BlazorServerMessageReceiver : ComponentBase, IAsyncDisposable
{
    private CancellationTokenSource? cts;
    private Task? readTask;

    [Inject]
    private ClientEventChannel EventChannel { get; set; } = default!;

    [Inject]
    private MessageDeserializer MessageDeserializer { get; set; } = default!;

    [Inject]
    private IMessageToActionMapper MessageToActionMapper { get; set; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    [Inject]
    private ILogger<BlazorServerMessageReceiver> Logger { get; set; } = default!;

    /// <inheritdoc />
    [SuppressMessage(
        "Usage",
        "VSTHRD003:Avoid awaiting foreign Tasks",
        Justification = "Task is created and managed within this component")]
    public async ValueTask DisposeAsync()
    {
        if (this.cts is not null)
        {
            await this.cts.CancelAsync();
            this.cts.Dispose();
        }

        if (this.readTask is not null)
        {
            try
            {
                await this.readTask;
            }
            catch (OperationCanceledException)
            {
                // Expected on disposal
            }
        }

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.cts = new CancellationTokenSource();
        this.readTask = this.ReadMessagesAsync(this.cts.Token);
    }

    private async Task ReadMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in this.EventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await this.ProcessMessageAsync(message);
            }
        }
        catch (OperationCanceledException ex)
        {
            this.Logger.LogDebug(ex, "Message reader cancelled");
        }
    }

    private Task ProcessMessageAsync(ClientMessage message)
    {
        var (messageBody, correlationId) = this.MessageDeserializer.Deserialize(message);
        if (messageBody is null)
        {
            return Task.CompletedTask;
        }

        var action = this.MessageToActionMapper.Map(messageBody, correlationId ?? Guid.Empty);
        if (action is null)
        {
            this.Logger.LogInformation("No action mapped to {@MessageBody}", messageBody);
            return Task.CompletedTask;
        }

        this.Dispatcher.Dispatch(action);
        this.Logger.LogInformation(
            "Action Type {Action} dispatched (Message Body: {@MessageBody})",
            action.GetType().Name,
            messageBody);

        return Task.CompletedTask;
    }
}
