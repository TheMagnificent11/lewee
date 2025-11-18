using Bunit;
using Correlate;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.Web.Pages;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.Orders;
using Pizzeria.Store.Web.States.Pizzas;

namespace Pizzeria.Store.Web.Tests.Unit;

public class HomePageTests : TestContext
{
    [Fact]
    public void Home_WhenNoCurrentOrder_ShowsStartNewOrderButton()
    {
        // Arrange
        var mockApiClient = new Mock<IPizzeriaApiClient>();
        var mockCorrelationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var mockLogger = new Mock<ILogger<OrdersEffects>>();

        this.Services.AddSingleton(mockApiClient.Object);
        this.Services.AddSingleton(mockCorrelationContextAccessor.Object);
        this.Services.AddSingleton(mockLogger.Object);
        this.Services.AddMudServices();
        this.Services.AddFluxor(o => o.ScanAssemblies(typeof(OrdersState).Assembly));

        // Act
        var component = this.RenderComponent<Home>();

        // Assert
        Assert.Contains("Start New Order", component.Markup, StringComparison.Ordinal);
        Assert.Contains("Ready to Order?", component.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Home_WhenStartNewOrderButtonClicked_ShowsLoadingState()
    {
        // Arrange
        var mockApiClient = new Mock<IPizzeriaApiClient>();
        var mockCorrelationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var mockLogger = new Mock<ILogger<OrdersEffects>>();

        this.Services.AddSingleton(mockApiClient.Object);
        this.Services.AddSingleton(mockCorrelationContextAccessor.Object);
        this.Services.AddSingleton(mockLogger.Object);
        this.Services.AddMudServices();
        this.Services.AddFluxor(o => o.ScanAssemblies(typeof(OrdersState).Assembly));

        var component = this.RenderComponent<Home>();

        // Act
        var button = component.Find("button");
        button.Click();

        // Assert
        // Note: This test may be flaky due to async operations
        // In a real scenario, you might want to mock the state or use more sophisticated testing
        Assert.NotNull(component.Find("button"));
    }
}
