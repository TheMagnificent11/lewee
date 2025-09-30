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
    public async Task CreateOrder_SendsSignalRMessage()
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
}
