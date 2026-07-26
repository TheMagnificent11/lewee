using Microsoft.Playwright;

namespace Lewee.Playwright;

/// <summary>
/// PlaywrightPage Extension Methods
/// </summary>
public static class PlaywrightExtensions
{
    /// <summary>
    /// Create Playwright Page asynchronously
    /// </summary>
    /// <param name="playwright">Playwright instance</param>
    /// <returns>A Task representing the asynchronous operation, with a PlaywrightPage as the result</returns>
    public static async Task<PlaywrightPage> CreatePlaywritePageAsync(this IPlaywright playwright)
    {
        ArgumentNullException.ThrowIfNull(playwright);

        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
        });

        var page = await context.NewPageAsync();

        return new PlaywrightPage(browser, context, page);
    }
}
