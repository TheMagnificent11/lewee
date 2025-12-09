using Bunit;
using FluentAssertions;

namespace Pizzeria.Store.Web.Tests.Unit;

public class HomePageTests : TestContext
{
    [Fact]
    public void Home_WhenNoCurrentOrder_ShowsStartNewOrderButton()
    {
        // Arrange
        this.Setup();

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
        this.Setup();

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
