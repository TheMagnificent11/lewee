using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Playwright;
using Pizzeria.Common;
using Pizzeria.Store.Api.Customers;
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
    public async Task Should_CreateCustomer_When_ValidUsernameAndPasswordProvided()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var request = new CreateCustomerRequest { Username = username, Password = password };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var customer = await this.factory.GetLatestCustomerAsync();
        customer.Should().NotBeNull();
        customer.ExternalId.Should().NotBeNullOrEmpty();
        customer.Id.Should().NotBeEmpty();

        // Verify the user was created in Keycloak by getting their ID
        var keycloakUserId = await this.factory.GetKeycloakUserIdAsync(username);
        customer.ExternalId.Should().Be(keycloakUserId);
    }

    [Fact]
    public async Task Should_RetrieveCustomerByExternalId_When_CustomerExists()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";
        var request = new CreateCustomerRequest { Username = username, Password = password };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var keycloakUserId = await this.factory.GetKeycloakUserIdAsync(username);
        var customer = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId);
        customer.Should().NotBeNull();
        customer.ExternalId.Should().Be(keycloakUserId);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_UsernameIsEmpty()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var request = new CreateCustomerRequest { Username = string.Empty, Password = "TestPassword123!" };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_PasswordIsTooShort()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username = $"testuser-{Guid.NewGuid()}";
        var request = new CreateCustomerRequest { Username = username, Password = "Short1!" }; // Less than 8 characters

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_CreateMultipleCustomers_When_DifferentUsernamesProvided()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username1 = $"testuser-{Guid.NewGuid()}";
        var username2 = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";

        // Act
        using var response1 = await httpClient.PostAsJsonAsync(
            Endpoints.StoreApi.Customers,
            new CreateCustomerRequest { Username = username1, Password = password });
        await this.WaitForDomainEventsToBeDispatchedAsync();

        using var response2 = await httpClient.PostAsJsonAsync(
            Endpoints.StoreApi.Customers,
            new CreateCustomerRequest { Username = username2, Password = password });
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response1.EnsureSuccessStatusCode();
        response2.EnsureSuccessStatusCode();

        var keycloakUserId1 = await this.factory.GetKeycloakUserIdAsync(username1);
        var keycloakUserId2 = await this.factory.GetKeycloakUserIdAsync(username2);

        var customer1 = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId1);
        var customer2 = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId2);

        customer1.Should().NotBeNull();
        customer2.Should().NotBeNull();
        customer1.Id.Should().NotBe(customer2.Id);
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
    public async Task Should_ShowErrorMessage_When_UsernameIsEmpty_InBrowser()
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
