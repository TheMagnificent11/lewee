using FluentAssertions;
using Microsoft.Playwright;
using Pizzeria.Store.Web.Layout;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class CustomerAuthTests : PizzeriaTests
{
    public CustomerAuthTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_BeAbleToSignInAndSignOut()
    {
        // Arrange
        var webClientUrl = await this.factory.GetWebClientBaseUrlAsync();
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        var playwright = await this.factory.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var page = await browser.NewPageAsync();

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

            // Step 2: Wait for redirect back to the app (home page)
            await page.WaitForURLAsync($"{webClientUrl}/**", new PageWaitForURLOptions { Timeout = 60000 });

            // Verify we're on the home page
            page.Url.Should().StartWith(webClientUrl);
            var pageContent = await page.ContentAsync();
            pageContent.Should().Contain("Lewee Pizzeria");

            // Step 3: Sign out the user
            var signOutButton = page.Locator(MainLayout.SignOutButtonSelector);

            // Click sign-out and wait for load state
            await signOutButton.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

            // After sign-out completes, we should eventually be on Keycloak login
            // Wait a bit for redirect to complete
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Verify we're on the Keycloak sign-in page
            var urlAfterSignOut = page.Url;
            urlAfterSignOut.Should().Contain("auth", "URL should contain 'auth' indicating Keycloak");

            // Step 4: Sign back in - wait for username field to be visible
            await page.WaitForSelectorAsync("#username", new PageWaitForSelectorOptions { Timeout = 15000 });
            await page.FillAsync("#username", username);
            await page.FillAsync("#password", password);

            // Try to find and click the sign-in button
            // Keycloak might use different button types, so try multiple approaches
            try
            {
                // First try: input type=submit
                await page.ClickAsync("input[type='submit']");
            }
            catch
            {
                try
                {
                    // Second try: button with type=submit
                    await page.ClickAsync("button[type='submit']");
                }
                catch
                {
                    // Third try: look for button with text "Sign In"
                    await page.ClickAsync("text=Sign In");
                }
            }

            // Step 5: Wait for redirect back to the home page
            await page.WaitForURLAsync($"{webClientUrl}/**", new PageWaitForURLOptions { Timeout = 60000 });

            // Verify we're on the home page again
            page.Url.Should().StartWith(webClientUrl);
            pageContent = await page.ContentAsync();
            pageContent.Should().Contain("Lewee Pizzeria");

            // Step 6: Sign out again to verify it works consistently
            signOutButton = page.Locator(MainLayout.SignOutButtonSelector);

            await signOutButton.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

            await Task.Delay(TimeSpan.FromSeconds(3));

            // Verify final sign-out redirected to Keycloak
            var finalUrl = page.Url;
            finalUrl.Should().Contain("auth");
        }
        finally
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
