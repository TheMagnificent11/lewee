using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
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
    public void MainLayout_WhenSignOutButtonClicked_NavigatesToLogoutEndpoint()
    {
        // Arrange
        this.Setup();

        var component = this.RenderComponent<MainLayout>();

        // Act
        var signOutButton = component.Find("button[aria-label='sign-out']");
        signOutButton.Click();

        // Assert
        var navigationManager = this.Services.GetRequiredService<NavigationManager>();
        navigationManager.Uri.Should().Contain("/logout");
    }
}
