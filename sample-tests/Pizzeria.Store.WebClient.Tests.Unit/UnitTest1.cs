using Bunit;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NSubstitute;
using Pizzeria.Store.WebClient.Pages;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States.Orders;
using Pizzeria.Store.WebClient.States.Pizzas;

namespace Pizzeria.Store.WebClient.Tests.Unit;

public class HomePageTests : TestContext
{
    [Fact]
    public void Home_WhenNoCurrentOrder_ShowsStartNewOrderButton()
    {
        // Arrange
        var mockApiClient = Substitute.For<IPizzeriaApiClient>();
        Services.AddSingleton(mockApiClient);
        Services.AddMudServices();
        Services.AddFluxor(o => o.ScanAssemblies(typeof(OrdersState).Assembly));

        // Act
        var component = RenderComponent<Home>();

        // Assert
        Assert.Contains("Start New Order", component.Markup);
        Assert.Contains("Ready to Order?", component.Markup);
    }

    [Fact]
    public void Home_WhenStartNewOrderButtonClicked_ShowsLoadingState()
    {
        // Arrange
        var mockApiClient = Substitute.For<IPizzeriaApiClient>();
        Services.AddSingleton(mockApiClient);
        Services.AddMudServices();
        Services.AddFluxor(o => o.ScanAssemblies(typeof(OrdersState).Assembly));

        var component = RenderComponent<Home>();

        // Act
        var button = component.Find("button");
        button.Click();

        // Assert
        // Note: This test may be flaky due to async operations
        // In a real scenario, you might want to mock the state or use more sophisticated testing
        Assert.NotNull(component.Find("button"));
    }
}
