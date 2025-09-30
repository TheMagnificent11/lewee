using Fluxor;
using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Tests.Integration;

/// <summary>
/// Test client that connects to the test server and receives SignalR messages
/// </summary>
public sealed class TestClient : IDisposable
{
    private readonly ServiceProvider serviceProvider;
    private readonly IMessageReceiver messageReceiver;
    private readonly ILogger<TestClient> logger;

    public TestClient(string serverBaseUrl, ILoggerFactory loggerFactory)
    {
        var services = new ServiceCollection();
        
        // Configure Lewee Blazor with the test server URL
        services.ConfigureLeweeBlazor<MessageToActionMapper>(serverBaseUrl, useReduxDevTools: false);
        
        // Add logger factory
        services.AddSingleton(loggerFactory);
        services.AddLogging();
        
        // Configure Fluxor
        services.AddFluxor(options => options
            .ScanAssemblies(typeof(TestClient).Assembly));
        
        serviceProvider = services.BuildServiceProvider();
        
        // Get required services
        messageReceiver = serviceProvider.GetRequiredService<IMessageReceiver>();
        logger = serviceProvider.GetRequiredService<ILogger<TestClient>>();
        
        logger.LogInformation("Test client created with server URL: {ServerUrl}", serverBaseUrl);
    }

    public async Task ConnectAsync()
    {
        logger.LogInformation("Connecting test client to server...");
        await messageReceiver.StartAsync();
        logger.LogInformation("Test client connected successfully");
    }

    public async Task DisconnectAsync()
    {
        logger.LogInformation("Disconnecting test client from server...");
        await messageReceiver.StopAsync();
        logger.LogInformation("Test client disconnected");
    }

    public void Dispose()
    {
        messageReceiver?.StopAsync().GetAwaiter().GetResult();
        serviceProvider?.Dispose();
    }
}
