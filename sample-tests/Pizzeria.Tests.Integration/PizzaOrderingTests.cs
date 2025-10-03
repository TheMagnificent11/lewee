using FluentAssertions;
using Pizzeria.Common;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class PizzaOrderingTests : PizzeriaTests
{
    public PizzaOrderingTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateOrder_When_OrderIsPlacedAsync()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.StoreApi.Orders);

        // Act
        using var response = await httpClient.SendAsync(request);
        await Task.Delay(TimeSpan.FromSeconds(3)); // Allow some time for the event to be processed

        // Assert
        response.EnsureSuccessStatusCode();

        var order = await this.factory.GetLatestOrderAsync();
        order.Should().NotBeNull();

        var orderProjection = await this.factory.GetQueryProjectionAsync<OrderQueryProjection>(order.Id.ToString());

        orderProjection.Should().NotBeNull();
        orderProjection.Order.Should().NotBeNull();
        orderProjection.Order.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task Should_AddPizzaToOrder_When_PizzaIsAddedAsync()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        using var createOrderRequest = new HttpRequestMessage(HttpMethod.Post, Endpoints.StoreApi.Orders);
        using var createOrderResponse = await httpClient.SendAsync(createOrderRequest);

        createOrderResponse.EnsureSuccessStatusCode();
        var order = await this.factory.GetLatestOrderAsync();
        order.Should().NotBeNull();
        using var addPizzaRequest = new HttpRequestMessage(
            HttpMethod.Put,
            Endpoints.StoreApi.GetAddPizzaToOrderEndpoint(order.Id, Menu.PizzaIds.QuattroFormaggi));

        // Act
        using var addPizzaResponse = await httpClient.SendAsync(addPizzaRequest);

        // Assert
        addPizzaResponse.EnsureSuccessStatusCode();

        order = await this.factory.GetOrderAsync(order.Id);
        order.Should().NotBeNull();
        order.Pizzas.Should().NotBeNull();
        order.Pizzas.Should().ContainSingle();

        var pizzaInOrder = order.Pizzas.First();
        pizzaInOrder.PizzaId.Should().Be(Menu.PizzaIds.QuattroFormaggi);
        pizzaInOrder.Quantity.Should().Be(1);
    }
}
