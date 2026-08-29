using FluentAssertions;
using Lewee.Playwright;
using Microsoft.Playwright;
using Pizzeria.Store.Components;
using Pizzeria.Store.Domain;
using Pizzeria.Tests.Integration.Infrastructure;
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
    public async Task Should_CreateOrder_And_NavigateToOrderPage_When_StartOrderButtonIsClicked()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        // Act
        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);

        playwrightPage.Page.ShouldHaveBannerHeading();

        await playwrightPage.Page.WaitForSelectorAsync(Home.Selectors.StartOrderButton, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(Home.Selectors.StartOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        try
        {
            // Wait for URL to contain /orders/ (indicating successful navigation to order page)
            await playwrightPage.Page.WaitForURLAsync(
                url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
                new PageWaitForURLOptions { Timeout = 30000 });

            // Assert - verify we're on an order details page
            var currentUrl = playwrightPage.Page.Url;
            currentUrl.Should().Contain("/orders/", "the app should navigate to the order page after receiving the SSE message");

            // Verify the page shows order-related content
            await playwrightPage.Page.WaitForSelectorAsync(
                Pizzeria.Store.Components.Order.Selectors.PizzaMenuHeading,
                new PageWaitForSelectorOptions { Timeout = 30000 });
            var orderPageContent = await playwrightPage.Page.ContentAsync();
            orderPageContent.Should().Contain("Pizza Menu", "the order page should show the pizza menu");
        }
        catch (TimeoutException ex)
        {
            var currentContent = await playwrightPage.Page.ContentAsync();
            var currentUrl = playwrightPage.Page.Url;

            var error = $"""
                    Timed out waiting for navigation to order page.
                    This indicates the SSE message was not received.
                    Current URL: {currentUrl}.
                    Page contains error: {currentContent.Contains("error", StringComparison.OrdinalIgnoreCase)}
                    """;

            throw new InvalidOperationException(error, ex);
        }

        // Assert
        var order = await this.Factory.GetLatestOrderAsync();
        order.Should().NotBeNull();

        var orderProjection = await this.Factory.GetQueryProjectionAsync<OrderQueryProjection>(order.Id.ToString());

        orderProjection.Should().NotBeNull();
        orderProjection.Order.Should().NotBeNull();
        orderProjection.Order.Id.Should().Be(order.Id);
    }
}
