using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class CustomerSignUpTests : PizzeriaTests
{
    public CustomerSignUpTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateCustomer_When_UserRegistersViaKeycloak()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        var playwright = await this.Factory.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        // Create a browser context that ignores HTTPS errors (needed for dev certificates)
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
        });
        var page = await context.NewPageAsync();

        try
        {
            // Act - Navigate to the web client (which should redirect to Keycloak login)
            await page.GotoAsync(webClientUrl);

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

            // Wait a bit for the User entity to be created via the OnTokenValidated event
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Wait for domain events to be dispatched
            await this.WaitForDomainEventsToBeDispatchedAsync();

            // Assert - Verify the user was created in the database
            var keycloakUserId = await this.Factory.GetKeycloakUserIdAsync(username);
            var customer = await this.Factory.GetCustomerByExternalIdAsync(keycloakUserId);
            customer.Should().NotBeNull();
            customer.ExternalId.Should().Be(keycloakUserId);
            customer.Id.Should().NotBeEmpty();
        }
        finally
        {
            await page.CloseAsync();
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }

    [Fact]
    public async Task Should_NavigateToHomePage_When_UserSuccessfullyRegisters()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        var playwright = await this.Factory.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });

        // Create a browser context that ignores HTTPS errors (needed for dev certificates)
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
        });
        var page = await context.NewPageAsync();

        try
        {
            // Act - Navigate to the web client
            await page.GotoAsync(webClientUrl);

            // Wait for Keycloak login page and click Register
            await page.WaitForSelectorAsync("text=Register", new PageWaitForSelectorOptions { Timeout = 30000 });
            await page.ClickAsync("text=Register");

            // Fill registration form
            await page.WaitForSelectorAsync("#firstName", new PageWaitForSelectorOptions { Timeout = 30000 });
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

            // Verify we can see content from the authenticated page
            // (This confirms the user is authenticated and not stuck on a redirect loop)
            var pageContent = await page.ContentAsync();
            pageContent.Should().Contain("Lewee Pizzeria");
        }
        finally
        {
            await page.CloseAsync();
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
