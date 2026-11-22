# Lewee.Playwright

This package provides helper utilities for writing integration tests using [Microsoft Playwright](https://playwright.dev/dotnet/). It simplifies browser automation setup and provides FluentAssertions-based assertion extensions for page elements.

## Dependencies

- [Microsoft.Playwright](https://playwright.dev/dotnet/) - Browser automation library
- [FluentAssertions](https://fluentassertions.com/) - Fluent assertion library for .NET

## Features

- **Simplified Browser Setup**: Easy-to-use wrapper for creating Playwright pages with sensible defaults
- **Automatic Resource Cleanup**: `IAsyncDisposable` pattern ensures proper cleanup of browser, context, and page
- **FluentAssertions Integration**: Natural assertion syntax for verifying page elements

## Configuration

### Installing Playwright Browsers

Before running tests, you need to install the Playwright browsers:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

Or using the Playwright CLI:

```bash
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

## Usage

### Creating a Playwright Page

Use the `CreatePlaywritePageAsync` extension method to create a new browser page with default settings:

```cs
using Lewee.Playwright;
using Microsoft.Playwright;

await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

// Navigate to your application
await playwrightPage.Page.GotoAsync("https://localhost:5001");
```

The `PlaywrightPage` wrapper:

- Launches a headless Chromium browser
- Creates a new browser context with HTTPS errors ignored (useful for development certificates)
- Creates a new page ready for navigation
- Automatically disposes of all resources when disposed

### Asserting Page Elements

Use the `ShouldHave` extension method to verify elements exist on the page:

```cs
using Lewee.Playwright;

// Assert that the page contains specific elements
playwrightPage.Page.ShouldHave("h1.page-title");
playwrightPage.Page.ShouldHave("#login-button");
playwrightPage.Page.ShouldHave("[data-testid='order-summary']");
```

### Complete Test Example

```cs
using Lewee.Playwright;
using Microsoft.Playwright;
using Xunit;

public class HomePageTests : IAsyncLifetime
{
    private IPlaywright? playwright;
    private PlaywrightPage? playwrightPage;

    public async Task InitializeAsync()
    {
        this.playwright = await Playwright.CreateAsync();
        this.playwrightPage = await this.playwright.CreatePlaywritePageAsync();
    }

    public async Task DisposeAsync()
    {
        if (this.playwrightPage != null)
        {
            await this.playwrightPage.DisposeAsync();
        }

        this.playwright?.Dispose();
    }

    [Fact]
    public async Task HomePage_ShouldDisplayWelcomeMessage()
    {
        // Arrange & Act
        await this.playwrightPage!.Page.GotoAsync("https://localhost:5001");

        // Assert
        this.playwrightPage.Page.ShouldHave("h1");
        this.playwrightPage.Page.ShouldHave(".welcome-message");
    }
}
```

## Key Components

### PlaywrightPage

The [PlaywrightPage](./PlaywrightPage.cs) class is a wrapper around the Playwright browser, context, and page. It implements `IAsyncDisposable` to ensure proper cleanup of all browser resources.

**Properties:**

- `Page`: The underlying `IPage` instance for browser automation

### PlaywrightExtensions

The [PlaywrightExtensions](./PlaywrightExtensions.cs) class provides the `CreatePlaywritePageAsync` extension method for `IPlaywright` instances.

**Default Configuration:**

- **Headless**: `true` - Browser runs without a visible window
- **IgnoreHTTPSErrors**: `true` - Useful for development certificates

### PageExtensions

The [PageExtensions](./PageExtensions.cs) class provides FluentAssertions-based assertion methods for `IPage` instances.

**Methods:**

- `ShouldHave(selector)`: Asserts that an element matching the CSS selector exists on the page

## Integration with Test Frameworks

### xUnit

```cs
public class MyTests : IAsyncLifetime
{
    private IPlaywright? playwright;
    private PlaywrightPage? page;

    public async Task InitializeAsync()
    {
        this.playwright = await Playwright.CreateAsync();
        this.page = await this.playwright.CreatePlaywritePageAsync();
    }

    public async Task DisposeAsync()
    {
        if (this.page != null)
        {
            await this.page.DisposeAsync();
        }

        this.playwright?.Dispose();
    }
}
```

### NUnit

```cs
[TestFixture]
public class MyTests
{
    private IPlaywright playwright = null!;
    private PlaywrightPage page = null!;

    [SetUp]
    public async Task SetUp()
    {
        this.playwright = await Playwright.CreateAsync();
        this.page = await this.playwright.CreatePlaywritePageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await this.page.DisposeAsync();
        this.playwright.Dispose();
    }
}
```

## Best Practices

1. **Always dispose resources**: Use `await using` or implement `IAsyncLifetime`/`IAsyncDisposable` to ensure browser resources are cleaned up
2. **Use data-testid attributes**: Prefer `[data-testid='...']` selectors for more stable tests
3. **Wait for page load**: Use `await page.GotoAsync(url)` with appropriate wait options for dynamic content
4. **Run headless in CI**: The default headless configuration is optimal for CI/CD pipelines
