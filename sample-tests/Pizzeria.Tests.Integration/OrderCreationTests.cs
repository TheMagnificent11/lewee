using FluentAssertions;
using Lewee.Playwright;
using Microsoft.Playwright;
using Xunit;

namespace Pizzeria.Tests.Integration;

/// <summary>
/// Tests that verify order creation through the Blazor UI and SignalR message handling.
/// These tests confirm that:
/// 1. The API successfully creates an order
/// 2. A SignalR message is sent from the API to the Blazor app
/// 3. The Blazor app receives the SignalR message and navigates to the order details page
/// </summary>
[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class OrderCreationTests : PizzeriaTests
{
    public OrderCreationTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact(Skip = "Broken")]
    public async Task Should_NavigateToOrderPage_When_OrderIsCreatedViaSignalR()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        // Step 1: Register a new user via Keycloak
        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);

        // Verify that Keycloak redirect back to web app
        playwrightPage.Page.ShouldHaveBannerHeading();

        // Step 2: Click the "Start New Order" button
        // Wait for the button to be visible
        await playwrightPage.Page.WaitForSelectorAsync("button:has-text('Start New Order')", new PageWaitForSelectorOptions { Timeout = 30000 });

        // Click the Start New Order button
        await playwrightPage.Page.ClickAsync("button:has-text('Start New Order')");

        // Step 3: Wait for navigation to the order page
        // This navigation only happens if the SignalR message is received
        // The app navigates to /orders/{orderId} when it receives the OrderDto via SignalR
        try
        {
            // Wait for URL to contain /orders/ (indicating successful navigation to order page)
            await playwrightPage.Page.WaitForURLAsync(
                url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
                new PageWaitForURLOptions { Timeout = 30000 });

            // Assert - verify we're on an order details page
            var currentUrl = playwrightPage.Page.Url;
            currentUrl.Should().Contain("/orders/", "the app should navigate to the order page after receiving the SignalR message");

            // Verify the page shows order-related content
            var orderPageContent = await playwrightPage.Page.ContentAsync();
            orderPageContent.Should().Contain("Pizza Menu", "the order page should show the pizza menu");
        }
        catch (TimeoutException ex)
        {
            var currentContent = await playwrightPage.Page.ContentAsync();
            var currentUrl = playwrightPage.Page.Url;

            var error = $"""
                    Timed out waiting for navigation to order page.
                    This indicates the SignalR message was not received.
                    Current URL: {currentUrl}.
                    Page contains error: {currentContent.Contains("error", StringComparison.OrdinalIgnoreCase)}
                    """;

            throw new InvalidOperationException(error, ex);
        }
    }
}
