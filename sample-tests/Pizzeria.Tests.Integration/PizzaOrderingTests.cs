using FluentAssertions;
using Pizzeria.Common;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class PizzaOrderingTests : PizzeriaTests
{
    private const string SkipReason = "Issues authorizing user when calling Pizza Store API";

    public PizzaOrderingTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact(Skip = SkipReason)]
    public async Task Should_CreateOrder_When_OrderIsPlacedAsync()
    {
        // Arrange
        using var httpClient = await this.Factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);

        // TODO: TNeed to register a customer user first and obtain a valid JWT for that user to call the Pizza Store API.
        var token = await this.Factory.GetJwtAsync("user", "password");
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.StoreApi.Orders);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        using var response = await httpClient.SendAsync(request);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var order = await this.Factory.GetLatestOrderAsync();
        order.Should().NotBeNull();

        var orderProjection = await this.Factory.GetQueryProjectionAsync<OrderQueryProjection>(order.Id.ToString());

        orderProjection.Should().NotBeNull();
        orderProjection.Order.Should().NotBeNull();
        orderProjection.Order.Id.Should().Be(order.Id);
    }

    [Fact(Skip = SkipReason)]
    public async Task Should_AddPizzaToOrder_When_PizzaIsAddedAsync()
    {
        // Arrange
        using var httpClient = await this.Factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);

        // TODO: TNeed to register a customer user first and obtain a valid JWT for that user to call the Pizza Store API.
        var token = await this.Factory.GetJwtAsync("user", "password");

        using var createOrderRequest = new HttpRequestMessage(HttpMethod.Post, Endpoints.StoreApi.Orders);
        createOrderRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var createOrderResponse = await httpClient.SendAsync(createOrderRequest);

        createOrderResponse.EnsureSuccessStatusCode();
        var order = await this.Factory.GetLatestOrderAsync();
        order.Should().NotBeNull();

        using var addPizzaRequest = new HttpRequestMessage(
            HttpMethod.Put,
            Endpoints.StoreApi.GetAddPizzaToOrderEndpoint(order.Id, Menu.PizzaIds.QuattroFormaggi));
        addPizzaRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        using var addPizzaResponse = await httpClient.SendAsync(addPizzaRequest);

        // Assert
        addPizzaResponse.EnsureSuccessStatusCode();

        order = await this.Factory.GetOrderAsync(order.Id);
        order.Should().NotBeNull();
        order.Pizzas.Should().NotBeNull();
        order.Pizzas.Should().ContainSingle();

        var pizzaInOrder = order.Pizzas.First();
        pizzaInOrder.PizzaId.Should().Be(Menu.PizzaIds.QuattroFormaggi);
        pizzaInOrder.Quantity.Should().Be(1);
    }
}
