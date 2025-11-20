using Bunit;
using FluentAssertions;
using Pizzeria.Store.Web.Layout;

namespace Pizzeria.Store.Web.Tests.Unit;

public class MainLayoutTests : TestContext
{
    [Fact]
    public void MainLayout_WhenRendered_ShowsAppBarWithTitle()
    {
        // Arrange
        this.Setup();

        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("Lewee Pizzeria");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutButton()
    {
        // Arrange
        this.Setup();

        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("aria-label=\"sign-out\"");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutFormWithCorrectAction()
    {
        // Arrange
        this.Setup();

        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain($"action=\"{Routes.SignOut}\"");
        component.Markup.Should().Contain("method=\"post\"");
    }
}
