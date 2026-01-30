using System.Diagnostics.CodeAnalysis;
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
                using var reader = new StreamReader(stream);

                await this.ProcessEventStreamAsync(reader, cancellationToken);
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
    }

    private async Task ProcessEventStreamAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        var dataBuilder = new System.Text.StringBuilder();

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null)
            {
                break;
            }

            if (string.IsNullOrEmpty(line))
            {
                if (dataBuilder.Length > 0)
                {
                    this.ProcessEvent(dataBuilder.ToString());
                    dataBuilder.Clear();
                }

                continue;
            }

            if (line.StartsWith("data:", StringComparison.Ordinal))
            {
                if (dataBuilder.Length > 0)
                {
                    dataBuilder.AppendLine();
                }

                dataBuilder.Append(line[5..].Trim());
            }
        }
    }

    private void ProcessEvent(string data)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ClientMessage>(data);
            if (message != null)
            {
                this.RaiseMessageReceived(message);
            }
        }
        catch (JsonException ex)
        {
            this.logger.LogSseDeserializationError(ex);
        }
    }
}
