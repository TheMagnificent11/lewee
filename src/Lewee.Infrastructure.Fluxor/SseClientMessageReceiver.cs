using System.Diagnostics.CodeAnalysis;
using System.Net.ServerSentEvents;
using System.Text.Json;
using Lewee.Common;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// SSE Client Message Receiver
/// </summary>
/// <remarks>
/// Receives server-sent events via HttpClient streaming.
/// </remarks>
[SuppressMessage(
    "Usage",
    "VSTHRD003:Avoid awaiting foreign Tasks",
    Justification = "Task is started within this context in StartAsync")]
[SuppressMessage(
    "Reliability",
    "CA2213:Disposable fields should be disposed",
    Justification = "CancellationTokenSource is disposed in StopAsync")]
public class SseClientMessageReceiver : IAsyncDisposable
{
    private readonly HttpClient httpClient;
    private readonly ILogger<SseClientMessageReceiver> logger;

    private readonly JsonSerializerOptions sseJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private CancellationTokenSource? cts;
    private Task? listeningTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="SseClientMessageReceiver"/> class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for SSE endpoint</param>
    /// <param name="logger">Logger</param>
    public SseClientMessageReceiver(
        HttpClient httpClient,
        ILogger<SseClientMessageReceiver> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    /// <summary>
    /// Event raised when a client message is received
    /// </summary>
    public event EventHandler<ClientMessageEventArgs>? OnMessageReceived;

    /// <summary>
    /// Starts listening for server-sent events
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the start operation</returns>
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        this.cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        this.listeningTask = this.ListenForEventsAsync(this.cts.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops listening for server-sent events
    /// </summary>
    /// <returns>Task</returns>
    public virtual async Task StopAsync()
    {
        if (this.cts != null)
        {
            await this.cts.CancelAsync();
            this.cts.Dispose();
            this.cts = null;
        }

        if (this.listeningTask != null)
        {
            try
            {
                await this.listeningTask.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
            catch (OperationCanceledException)
            {
                // Expected on cancellation
            }
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        await this.StopAsync();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Raises the OnMessageReceived event
    /// </summary>
    /// <param name="message">Client message</param>
    protected void RaiseMessageReceived(ClientMessage message)
    {
        this.OnMessageReceived?.Invoke(this, new ClientMessageEventArgs(message));
    }

    private async Task ListenForEventsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "/events");
                request.Headers.Accept.Add(new("text/event-stream"));

                using var response = await this.httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                await foreach (var item in GetSseDataAsync(stream, cancellationToken))
                {
                    this.ProcessEvent(item);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                this.logger.LogSseConnectionError(ex);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        static IAsyncEnumerable<string> GetSseDataAsync(Stream stream, CancellationToken cancellationToken)
        {
            return SseParser.Create(stream)
                .EnumerateAsync(cancellationToken)
                .Select(x => x.Data)
                .Where(static x => !string.IsNullOrEmpty(x))
                .Select(static x => x!);
        }
    }

    private void ProcessEvent(string data)
    {
        try
        {
            // Deserialize the wrapper object that contains the client message
            // Note: The server sends SseItem<ClientMessage> serialized as JSON with camelCase property names
            var wrapper = JsonSerializer.Deserialize<SseItemWrapper>(data, this.sseJsonOptions);
            if (wrapper?.Data == null)
            {
                this.logger.LogSseEventDataNull();
                return;
            }

            this.RaiseMessageReceived(wrapper.Data);
        }
        catch (JsonException ex)
        {
            this.logger.LogSseDeserializationError(ex);
        }
    }

    /// <summary>
    /// Wrapper class for deserializing SSE items from camelCase JSON
    /// </summary>
    /// <remarks>
    /// This is needed because System.Net.ServerSentEvents.SseItem uses PascalCase properties
    /// but ASP.NET Core serializes with camelCase when using default JSON options
    /// </remarks>
    internal sealed class SseItemWrapper
    {
        public ClientMessage? Data { get; set; }
        public string? EventType { get; set; }
        public string? EventId { get; set; }
        public int? ReconnectionInterval { get; set; }
    }
}
