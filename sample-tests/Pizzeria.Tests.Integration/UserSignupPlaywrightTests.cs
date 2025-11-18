using FluentAssertions;
using Microsoft.Playwright;
using Pizzeria.Common;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class UserSignupPlaywrightTests : PizzeriaTests
{
    public UserSignupPlaywrightTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_SignUpNewUser_And_NavigateToOrderPage_When_ValidCredentialsProvided()
    {
        // Arrange
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";

        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        try
        {
            var webClientBaseUrl = await this.factory.GetWebClientBaseUrlAsync();

            // Act - Navigate to sign up page
            await page.GotoAsync($"{webClientBaseUrl}/signup");

            // Wait for the page to load
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Fill in the sign-up form
            await page.FillAsync("input[label='Username']", username);
            await page.FillAsync("input[type='password']", password);

            // Click the sign-up button
            await page.ClickAsync("button:has-text('Sign Up')");

            // Wait for success message or navigation
            await page.WaitForURLAsync($"{webClientBaseUrl}/order", new PageWaitForURLOptions
            {
                Timeout = 30000, // 30 seconds timeout
            });

            // Assert - Check that we navigated to the order page
            page.Url.Should().Contain("/order");

            // Verify the user was created in the database
            await this.WaitForDomainEventsToBeDispatchedAsync();
            var keycloakUserId = await this.factory.GetKeycloakUserIdAsync(username);
            var customer = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId);
            customer.Should().NotBeNull();
            customer.ExternalId.Should().Be(keycloakUserId);
        }
        finally
        {
            await page.CloseAsync();
            await context.CloseAsync();
            await browser.CloseAsync();
            playwright.Dispose();
        }
    }

    [Fact]
    public async Task Should_ShowErrorMessage_When_UsernameIsEmpty()
    {
        // Arrange
        var password = "TestPassword123!";

        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        try
        {
            var webClientBaseUrl = await this.factory.GetWebClientBaseUrlAsync();

            // Act - Navigate to sign up page
            await page.GotoAsync($"{webClientBaseUrl}/signup");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Fill in only the password
            await page.FillAsync("input[type='password']", password);

            // Assert - Sign-up button should be disabled
            var signUpButton = page.Locator("button:has-text('Sign Up')");
            await signUpButton.WaitForAsync();
            var isDisabled = await signUpButton.IsDisabledAsync();
            isDisabled.Should().BeTrue();
        }
        finally
        {
            await page.CloseAsync();
            await context.CloseAsync();
            await browser.CloseAsync();
            playwright.Dispose();
        }
    }
}
