using FluentAssertions;
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

    [Fact]
    public async Task Should_NavigateToOrderPage_When_OrderIsCreatedViaSignalR()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        var playwright = await this.Factory.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
        });
        var page = await context.NewPageAsync();

        try
        {
            // Step 1: Navigate to the web client and register a new user
            await page.GotoAsync(webClientUrl);

            // Wait for Keycloak login page to load
            await page.WaitForSelectorAsync("text=Register", new PageWaitForSelectorOptions { Timeout = 30000 });

            // Click on Register link
            await page.ClickAsync("text=Register");

            // Wait for registration form
            await page.WaitForSelectorAsync("#firstName", new PageWaitForSelectorOptions { Timeout = 30000 });

            // Fill out registration form
            await page.FillAsync("#firstName", username);
            await page.FillAsync("#lastName", "User");
            await page.FillAsync("#email", email);
            await page.FillAsync("#username", username);
            await page.FillAsync("#password", password);
            await page.FillAsync("#password-confirm", password);

            // Submit registration
            await page.ClickAsync("input[type='submit']");

            // Wait for redirect back to the app (home page)
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

            // Verify we're on the home page
            var pageContent = await page.ContentAsync();
            pageContent.Should().Contain("Lewee Pizzeria");

            // Step 2: Click the "Start New Order" button
            // Wait for the button to be visible
            await page.WaitForSelectorAsync("button:has-text('Start New Order')", new PageWaitForSelectorOptions { Timeout = 30000 });

            // Click the Start New Order button
            await page.ClickAsync("button:has-text('Start New Order')");

            // Step 3: Wait for navigation to the order page
            // This navigation only happens if the SignalR message is received
            // The app navigates to /orders/{orderId} when it receives the OrderDto via SignalR
            try
            {
                // Wait for URL to contain /orders/ (indicating successful navigation to order page)
                await page.WaitForURLAsync(
                    url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
                    new PageWaitForURLOptions { Timeout = 30000 });

                // Assert - verify we're on an order details page
                var currentUrl = page.Url;
                currentUrl.Should().Contain("/orders/", "the app should navigate to the order page after receiving the SignalR message");

                // Verify the page shows order-related content
                var orderPageContent = await page.ContentAsync();
                orderPageContent.Should().Contain("Pizza Menu", "the order page should show the pizza menu");
            }
            catch (TimeoutException)
            {
                // If we timeout waiting for navigation, check if there's an error message
                var currentContent = await page.ContentAsync();
                var currentUrl = page.Url;

                // Fail with diagnostic information
                Assert.Fail(
                    $"Timed out waiting for navigation to order page. " +
                    $"This indicates the SignalR message was not received. " +
                    $"Current URL: {currentUrl}. " +
                    $"Page contains error: {currentContent.Contains("error", StringComparison.OrdinalIgnoreCase)}");
            }
        }
        finally
        {
            await page.CloseAsync();
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
