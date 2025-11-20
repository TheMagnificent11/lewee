using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.Web.Layout;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.Orders;

namespace Pizzeria.Store.Web.Tests.Unit;

public class MainLayoutTests : TestContext
{
    [Fact]
    public void MainLayout_WhenRendered_ShowsAppBarWithTitle()
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

        // Configure JSInterop for MudBlazor
        this.JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("Lewee Pizzeria");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutButton()
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

        // Configure JSInterop for MudBlazor
        this.JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("aria-label=\"sign-out\"");
    }

    [Fact]
    public void MainLayout_WhenSignOutButtonClicked_NavigatesToLogoutEndpoint()
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

        // Configure JSInterop for MudBlazor
        this.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = this.RenderComponent<MainLayout>();

        // Act
        var signOutButton = component.Find("button[aria-label='sign-out']");
        signOutButton.Click();

        // Assert
        var navigationManager = this.Services.GetRequiredService<NavigationManager>();
        navigationManager.Uri.Should().Contain("/logout");
    }
}
