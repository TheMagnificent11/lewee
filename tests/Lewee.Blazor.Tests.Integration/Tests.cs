using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[Collection(TestServerFixture.CollectionName)]
public sealed class Tests 
{
    private readonly TestServerFixture testServer;

    public Tests(TestServerFixture testServer)
    {
        this.testServer = testServer;
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Arrange
        using var client = this.testServer.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateOrder_StoresOrderInMemory()
    {
        // Arrange
        using var client = this.testServer.CreateClient();
        var request = new CreateOrderRequest("John Doe", "Margherita", 2);

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<PizzaOrder>();
        Assert.NotNull(order);
        Assert.Equal("John Doe", order.CustomerName);
        Assert.Equal("Margherita", order.PizzaType);
        Assert.Equal(2, order.Quantity);

        // Verify order was stored in memory
        Assert.True(TestServerFixture.Orders.ContainsKey(order.Id));
        var storedOrder = TestServerFixture.Orders[order.Id];
        Assert.Equal(order.Id, storedOrder.Id);
        Assert.Equal("John Doe", storedOrder.CustomerName);
    }
    
    [Fact]
    public async Task CreateOrder_ClientReceivesSignalRMessage()
    {
        // Arrange
        var serverUrl = this.testServer.GetServerUrl();
        var loggerFactory = this.testServer.GetLoggerFactory();
        
        using var testClient = new TestClient(serverUrl, loggerFactory);
        await testClient.ConnectAsync();
        
        using var httpClient = this.testServer.CreateClient();
        var request = new CreateOrderRequest("Jane Smith", "Pepperoni", 3);

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<PizzaOrder>();
        Assert.NotNull(order);
        
        // Wait a bit for SignalR message to be received
        await Task.Delay(500);
        
        // Verify the message was logged by MessageToActionMapper
        var logs = this.testServer.GetLogs(LogLevel.Information);
        var messageLog = logs.FirstOrDefault(log => 
            log.Message.Contains("SignalR message received") && 
            log.Message.Contains(nameof(PizzaOrder)));
        
        Assert.NotNull(messageLog);
        
        await testClient.DisconnectAsync();
    }
}
