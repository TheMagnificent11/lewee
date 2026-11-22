using Microsoft.Playwright;

namespace Lewee.Playwright;

/// <summary>
/// Playwright Page Wrapper
/// </summary>
public class PlaywrightPage : IAsyncDisposable
{
    private readonly IBrowser browser;
    private readonly IBrowserContext context;

    internal PlaywrightPage(IBrowser browser, IBrowserContext context, IPage page)
    {
        this.browser = browser;
        this.context = context;
        this.Page = page;
    }

    /// <summary>
    /// Gets the page
    /// </summary>
    public IPage Page { get; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await this.Page.CloseAsync();
        await this.context.CloseAsync();
        await this.browser.CloseAsync();

        GC.SuppressFinalize(this);
    }
}
