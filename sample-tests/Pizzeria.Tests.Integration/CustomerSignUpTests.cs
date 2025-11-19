using FluentAssertions;
using Microsoft.Playwright;
using Pizzeria.Common;
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
        var webClientUrl = await this.factory.GetWebClientBaseUrlAsync();
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        var playwright = await this.factory.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var page = await browser.NewPageAsync();

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

            // Wait for redirect back to the app
            await page.WaitForURLAsync($"{webClientUrl}/**", new PageWaitForURLOptions { Timeout = 60000 });

            // Wait a bit for the User entity to be created via the OnTokenValidated event
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Wait for domain events to be dispatched
            await this.WaitForDomainEventsToBeDispatchedAsync();

            // Assert - Verify the user was created in the database
            var keycloakUserId = await this.factory.GetKeycloakUserIdAsync(username);
            var customer = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId);
            customer.Should().NotBeNull();
            customer.ExternalId.Should().Be(keycloakUserId);
            customer.Id.Should().NotBeEmpty();
        }
        finally
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }

    [Fact]
    public async Task Should_NavigateToHomePage_When_UserSuccessfullyRegisters()
    {
        // Arrange
        var webClientUrl = await this.factory.GetWebClientBaseUrlAsync();
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var email = $"{username}@example.com";

        var playwright = await this.factory.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var page = await browser.NewPageAsync();

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

            // Wait for redirect back to the app
            await page.WaitForURLAsync($"{webClientUrl}/**", new PageWaitForURLOptions { Timeout = 60000 });

            // Assert - Verify user is on the home page (authenticated)
            page.Url.Should().StartWith(webClientUrl);

            // Verify we can see content from the authenticated page
            // (This confirms the user is authenticated and not stuck on a redirect loop)
            var pageContent = await page.ContentAsync();
            pageContent.Should().Contain("Lewee Pizzeria");
        }
        finally
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
