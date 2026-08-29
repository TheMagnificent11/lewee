using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Lewee.Playwright.Tests.Unit;

public sealed class PageExtensionsTests
{
    [Theory]
    [InlineData("net::ERR_NETWORK_CHANGED at http://localhost:41763/")]
    [InlineData("Navigation failed: net::ERR_CONNECTION_RESET")]
    [InlineData("net::ERR_CONNECTION_REFUSED at http://example.com")]
    [InlineData("Error: net::ERR_CONNECTION_CLOSED")]
    [InlineData("Failed with net::ERR_NETWORK_IO_SUSPENDED")]
    [InlineData("net::ERR_SOCKET_NOT_CONNECTED during request")]
    [InlineData("Request timed out: net::ERR_TIMED_OUT")]
    public void Should_ReturnTrue_When_ExceptionContainsTransientNetworkError(string message)
    {
        // Arrange
        var exception = new PlaywrightException(message);

        // Act
        var result = PageExtensions.IsTransientNetworkError(exception);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("net::ERR_NAME_NOT_RESOLVED")]
    [InlineData("Element not found")]
    [InlineData("Timeout waiting for selector")]
    [InlineData("Navigation to about:blank failed")]
    [InlineData("Page crashed")]
    [InlineData("")]
    public void Should_ReturnFalse_When_ExceptionDoesNotContainTransientNetworkError(string message)
    {
        // Arrange
        var exception = new PlaywrightException(message);

        // Act
        var result = PageExtensions.IsTransientNetworkError(exception);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ThrowArgumentNullException_When_ExceptionIsNull()
    {
        // Arrange
        PlaywrightException exception = null;

        // Act
        var act = () => PageExtensions.IsTransientNetworkError(exception);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("exception");
    }

    [Fact]
    public void MaxRetryDuration_Should_BeTenSeconds()
    {
        PageExtensions.MaxRetryDuration.Should().Be(TimeSpan.FromSeconds(10));
    }
}
