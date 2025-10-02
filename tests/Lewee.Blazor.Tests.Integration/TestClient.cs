using Lewee.Blazor.Messaging;
using Lewee.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace Lewee.Blazor.Tests.Integration;

public sealed class TestClient : IDisposable
{
    private readonly ServiceProvider serviceProvider;
    private readonly ILogger<TestClient> logger;
    private readonly FakeLogCollector fakeLogCollector;
    private readonly HubConnection hub;
    private readonly TestHttpClient httpClient;

    public TestClient(HttpClient httpClient)
    {
        this.httpClient = new TestHttpClient(httpClient);

        var services = new ServiceCollection();

        services
            .AddFakeLogging()
            .AddLeweeBlazorForTesting<MessageToActionMapper>(httpClient);

        this.serviceProvider = services.BuildServiceProvider();

        this.logger = this.serviceProvider.GetRequiredService<ILogger<TestClient>>();
        this.fakeLogCollector = this.serviceProvider.GetRequiredService<FakeLogCollector>();
        this.hub = this.serviceProvider.GetRequiredService<HubConnection>();

        var messageDeserializer = this.serviceProvider.GetRequiredService<MessageDeserializer>();
        var messageToActionMapper = this.serviceProvider.GetRequiredService<IMessageToActionMapper>();

        this.hub.On<ClientMessage>(nameof(ClientMessage), message =>
        {
            var (messageBody, correlationId) = messageDeserializer.Deserialize(message);
            if (messageBody == null)
            {
                return;
            }

            var action = messageToActionMapper.Map(messageBody, correlationId ?? Guid.Empty);
            if (action == null)
            {
                this.logger.LogInformation("No action mapped to {@MessageBody}", messageBody);
                return;
            }

            this.logger.LogInformation(
                "Action Type {Action} dispatched (Message Body: {@MessageBody})",
                action.GetType().Name,
                messageBody);
        });

        this.logger.LogInformation(
            "Test client created with server URL: {ServerUrl}",
            httpClient.BaseAddress!.AbsolutePath);
    }

    public async Task ConnectAsync()
    {
        this.logger.LogInformation("Connecting test client to server...");

        await this.hub.StartAsync();

        this.logger.LogInformation("Test client connected successfully");
    }

    public async Task DisconnectAsync()
    {
        this.logger.LogInformation("Disconnecting test client from server...");

        await this.hub.StopAsync();

        this.logger.LogInformation("Test client disconnected");
    }

    public async Task<bool> GetHealthAsync()
    {
        return await this.httpClient.GetHealthAsync();
    }

    public async Task<bool> CreatePizzaOrderAsync()
    {
        return await this.httpClient.CreatePizzaOrderAsync();
    }

    public IReadOnlyList<FakeLogRecord> GetLogs()
    {
        return this.fakeLogCollector.GetSnapshot();
    }

    public void Dispose()
    {
        this.serviceProvider?.Dispose();
    }
}
