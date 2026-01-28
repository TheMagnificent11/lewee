using Lewee.Playwright;
using Microsoft.Playwright;

namespace Pizzeria.Tests.Integration.Infrastructure;

internal static class PageExtensions
{
    public static async Task RegisterUserAsync(
        this IPage page,
        string webClientUrl,
        string username,
        string password,
        string email)
    {
        // Navigate to the web client (which should redirect to Keycloak login)
        // Use resilient navigation to handle transient network errors like net::ERR_NETWORK_CHANGED
        await page.GotoWithRetryAsync(webClientUrl);

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

        // Wait for redirect back to the app (use load state since URL may change due to HTTPS redirect)
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });
    }

    public static void ShouldHaveBannerHeading(this IPage page)
    {
        page.ShouldHave(Store.MainLayout.Selectors.BannerHeading);
    }
}
