using FluentAssertions;
using Microsoft.Playwright;
using Polly;
using Polly.Retry;

namespace Lewee.Playwright;

/// <summary>
/// IPage Extension Methods
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Maximum total wait time for retries (10 seconds)
    /// </summary>
    public static readonly TimeSpan MaxRetryDuration = TimeSpan.FromSeconds(10);

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

    private static readonly ResiliencePipeline NavigationResiliencePipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<PlaywrightException>(IsTransientNetworkError),
            MaxRetryAttempts = 10,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.FromMilliseconds(200),
            MaxDelay = TimeSpan.FromSeconds(5),
        })
        .AddTimeout(MaxRetryDuration)
        .Build();

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
    /// Navigate to a URL with retry logic for transient network errors.
    /// Uses exponential backoff with a maximum total wait time of 10 seconds.
    /// </summary>
    /// <param name="page">Playwright page</param>
    /// <param name="url">URL to navigate to</param>
    /// <param name="options">Optional navigation options</param>
    /// <returns>The response from the navigation, or null if the page navigated successfully</returns>
    public static async Task<IResponse?> GotoWithRetryAsync(
        this IPage page,
        string url,
        PageGotoOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrEmpty(url);

        return await NavigationResiliencePipeline.ExecuteAsync(
            async _ => await page.GotoAsync(url, options));
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
