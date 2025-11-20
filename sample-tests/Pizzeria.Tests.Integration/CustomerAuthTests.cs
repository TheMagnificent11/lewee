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
            await signOutButton.ClickAsync();

            // Wait for navigation to Keycloak login page
            // Use a URL pattern instead of a predicate to properly wait for navigation
            await page.WaitForURLAsync("**/auth/**", new PageWaitForURLOptions { Timeout = 30000 });

            // Verify we're on the Keycloak sign-in page
            var currentUrl = page.Url;
            currentUrl.Should().Contain("auth"); // Keycloak URL contains 'auth'
            currentUrl.Should().Contain("login"); // Login page

            // Wait for the sign-in form to be visible
            await page.WaitForSelectorAsync("#username", new PageWaitForSelectorOptions { Timeout = 10000 });

            // Step 5: Sign back in
            await page.FillAsync("#username", username);
            await page.FillAsync("#password", password);
            await page.ClickAsync("input[type='submit']");

            // Step 6: Wait for redirect back to the home page
            await page.WaitForURLAsync($"{webClientUrl}/**", new PageWaitForURLOptions { Timeout = 60000 });

            // Verify we're on the home page again
            page.Url.Should().StartWith(webClientUrl);
            pageContent = await page.ContentAsync();
            pageContent.Should().Contain("Lewee Pizzeria");

            // Step 7: Sign out again
            signOutButton = page.Locator(MainLayout.SignOutButtonSelector);
            await signOutButton.ClickAsync();

            // Wait for navigation to Keycloak login page
            await page.WaitForURLAsync("**/auth/**", new PageWaitForURLOptions { Timeout = 30000 });

            currentUrl = page.Url;
            currentUrl.Should().Contain("auth");
            currentUrl.Should().Contain("login");
        }
        finally
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
