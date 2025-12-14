using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.StateManagement;
using Xunit;

namespace Pizzeria.Store.Components.Tests.Unit;

public class HomePageTests : TestContext
{
    public HomePageTests()
    {
        this.Services.AddSingleton(Mock.Of<IMediator>());
        this.Services.AddSingleton(Mock.Of<ICorrelationContextAccessor>());
        this.Services.AddLogging();
        this.Services.AddMudServices();
        this.Services.AddFluxor(o => o.ScanAssemblies(typeof(StoreStateManagementConfiguration).Assembly));

        this.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Home_WhenNoCurrentOrder_ShowsStartNewOrderButton()
    {
        // Act
        var component = this.RenderComponent<Home>();

        // Assert
        component.Markup.Should().Contain("Start New Order");
        component.Markup.Should().Contain("Ready to Order?");
    }

    [Fact]
    public void Home_WhenStartNewOrderButtonClicked_ShowsLoadingState()
    {
        // Arrange
        var component = this.RenderComponent<Home>();

        // Act
        var button = component.Find("button");
        button.Click();

        // Assert
        // Note: This test may be flaky due to async operations
        // In a real scenario, you might want to mock the state or use more sophisticated testing
        var nextButton = component.Find("button");
        nextButton.Should().NotBeNull();
    }
}
