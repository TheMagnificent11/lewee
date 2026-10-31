using System.Collections.Concurrent;
using FluentAssertions;
using Lewee.Playwright;
using Microsoft.Playwright;
using Pizzeria.Ordering.Web;
using Pizzeria.Tests.Integration.Infrastructure;
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
        var requests = new ConcurrentBag<string>();
        var circuitRequests = new ConcurrentBag<string>();
        playwrightPage.Page.Request += (_, request) =>
        {
            requests.Add(request.Url);
            if (request.Url.Contains("/_blazor/negotiate", StringComparison.OrdinalIgnoreCase))
            {
                circuitRequests.Add(request.Url);
            }
        };

        // Register a new user via Keycloak and return to the requested app URL
        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);

        playwrightPage.Page.ShouldHaveBannerHeading();
        playwrightPage.Page.Url.Should().StartWith(webClientUrl);

        // Step 3: Sign out the user
        var signOutButton = playwrightPage.Page.Locator(MainLayout.Selectors.SignOutButton);

        // Click sign-out and wait for load state
        await signOutButton.ClickAsync();
        await playwrightPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

        await playwrightPage.Page.GotoAsync(webClientUrl);

        // Sign back in after the browser session has ended
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

        // Wait for redirect back to the original app route
        await playwrightPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

        playwrightPage.Page.ShouldHaveBannerHeading();
        playwrightPage.Page.Url.Should().StartWith(webClientUrl);

        signOutButton = playwrightPage.Page.Locator(MainLayout.Selectors.SignOutButton);

        await signOutButton.ClickAsync();
        await playwrightPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });

        await playwrightPage.Page.GotoAsync(webClientUrl);
        await playwrightPage.Page.WaitForSelectorAsync("#username", new PageWaitForSelectorOptions { Timeout = 15000 });

        circuitRequests.Should().BeEmpty("the WASM client must not establish a server-side circuit");
        requests.Should().Contain(request =>
            new Uri(request).AbsolutePath.Contains("/_api/bff/", StringComparison.OrdinalIgnoreCase));
        requests.Should().NotContain(request =>
            new Uri(request).AbsolutePath.Contains("/pizza-store-api/", StringComparison.OrdinalIgnoreCase)
            || new Uri(request).AbsolutePath.Contains("/auth-api/", StringComparison.OrdinalIgnoreCase));
    }
}
