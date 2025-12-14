using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.StateManagement.Orders;
using Xunit;

namespace Pizzeria.Store.Web.Tests.Unit;

public class MainLayoutTests : TestContext
{
    public MainLayoutTests()
    {
        this.Services.AddSingleton(Mock.Of<IMediator>());
        this.Services.AddSingleton(Mock.Of<ICorrelationContextAccessor>());
        this.Services.AddSingleton(Mock.Of<ILogger<StartOrderEffects>>());
        this.Services.AddMudServices();
        this.Services.AddFluxor(o => o.ScanAssemblies(typeof(OrderState).Assembly));

        this.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsAppBarWithTitle()
    {
        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("Lewee Pizzeria");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutButton()
    {
        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("aria-label=\"sign-out\"");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutFormWithCorrectAction()
    {
        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain($"action=\"{PageRoutes.SignOut}\"");
        component.Markup.Should().Contain("method=\"post\"");
    }
}
