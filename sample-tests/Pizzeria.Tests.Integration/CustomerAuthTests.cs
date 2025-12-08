using FluentAssertions;
using Lewee.Playwright;
using Microsoft.Playwright;
using Pizzeria.Store.Web;
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
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        // Step 1: Register a new user via Keycloak
        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);

        playwrightPage.Page.ShouldHaveBannerHeading();

        // Step 3: Sign out the user
        var signOutButton = playwrightPage.Page.Locator(MainLayout.SignOutButtonSelector);

        // Click sign-out and wait for load state
        await signOutButton.ClickAsync();
        await playwrightPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

        // After sign-out completes, we should eventually be on Keycloak login
        // Wait a bit for redirect to complete
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Verify we're on the Keycloak sign-in page
        var urlAfterSignOut = playwrightPage.Page.Url;
        urlAfterSignOut.Should().Contain("auth", "URL should contain 'auth' indicating Keycloak");

        // Step 4: Sign back in - wait for username field to be visible
        await playwrightPage.Page.WaitForSelectorAsync("#username", new PageWaitForSelectorOptions { Timeout = 15000 });
        await playwrightPage.Page.FillAsync("#username", username);
        await playwrightPage.Page.FillAsync("#password", password);

        // Try to find and click the sign-in button
        // Keycloak might use different button types, so try multiple approaches
        try
        {
            // First try: input type=submit
            await playwrightPage.Page.ClickAsync("input[type='submit']");
        }
        catch
        {
            try
            {
                // Second try: button with type=submit
                await playwrightPage.Page.ClickAsync("button[type='submit']");
            }
            catch
            {
                // Third try: look for button with text "Sign In"
                await playwrightPage.Page.ClickAsync("text=Sign In");
            }
        }

        // Step 5: Wait for redirect back to the home page
        await playwrightPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

        playwrightPage.Page.ShouldHaveBannerHeading();

        // Step 6: Sign out again to verify it works consistently
        signOutButton = playwrightPage.Page.Locator(MainLayout.SignOutButtonSelector);

        await signOutButton.ClickAsync();
        await playwrightPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

        await Task.Delay(TimeSpan.FromSeconds(3));

        // Verify final sign-out redirected to Keycloak
        var finalUrl = playwrightPage.Page.Url;
        finalUrl.Should().Contain("auth");
    }
}
