using FluentAssertions;
using Microsoft.Playwright;

namespace Lewee.Playwright;

/// <summary>
/// IPage Extension Methods
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Default number of retry attempts for resilient navigation
    /// </summary>
    public const int DefaultMaxRetries = 3;

    /// <summary>
    /// Default delay in milliseconds between retry attempts
    /// </summary>
    public const int DefaultRetryDelayMilliseconds = 1000;

    private static readonly string[] TransientNetworkErrors =
    [
        "net::ERR_NETWORK_CHANGED",
        "net::ERR_CONNECTION_RESET",
        "net::ERR_CONNECTION_REFUSED",
        "net::ERR_CONNECTION_CLOSED",
        "net::ERR_NETWORK_IO_SUSPENDED",
        "net::ERR_SOCKET_NOT_CONNECTED",
        "net::ERR_TIMED_OUT",
    ];

    /// <summary>
    /// Assert that the page contains an element with specified selector
    /// </summary>
    /// <param name="page">Playwright page</param>
    /// <param name="selector">CSS selector of the element</param>
    public static void ShouldHave(this IPage page, string selector)
    {
        ArgumentNullException.ThrowIfNull(page);

        var element = page.Locator(selector);

        element.Should().NotBeNull();
    }

    /// <summary>
    /// Navigate to a URL with retry logic for transient network errors
    /// </summary>
    /// <param name="page">Playwright page</param>
    /// <param name="url">URL to navigate to</param>
    /// <param name="options">Optional navigation options</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3)</param>
    /// <param name="retryDelayMilliseconds">Delay between retries in milliseconds (default: 1000)</param>
    /// <returns>The response from the navigation, or null if navigation was retried successfully</returns>
    public static async Task<IResponse?> GotoWithRetryAsync(
        this IPage page,
        string url,
        PageGotoOptions? options = null,
        int maxRetries = DefaultMaxRetries,
        int retryDelayMilliseconds = DefaultRetryDelayMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrEmpty(url);

        var lastException = default(PlaywrightException);

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await page.GotoAsync(url, options);
            }
            catch (PlaywrightException ex) when (IsTransientNetworkError(ex))
            {
                lastException = ex;

                if (attempt < maxRetries)
                {
                    await Task.Delay(retryDelayMilliseconds);
                }
            }
        }

        throw new PlaywrightException(
            $"Navigation to '{url}' failed after {maxRetries + 1} attempts due to transient network errors",
            lastException!);
    }

    /// <summary>
    /// Determines if a PlaywrightException is caused by a transient network error
    /// </summary>
    /// <param name="exception">The exception to check</param>
    /// <returns>True if the exception is a transient network error, false otherwise</returns>
    public static bool IsTransientNetworkError(PlaywrightException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var message = exception.Message;

        foreach (var error in TransientNetworkErrors)
        {
            if (message.Contains(error, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
