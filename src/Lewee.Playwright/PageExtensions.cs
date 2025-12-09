using FluentAssertions;
using Microsoft.Playwright;

namespace Lewee.Playwright;

/// <summary>
/// IPage Extension Methods
/// </summary>
public static class PageExtensions
{
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
}
