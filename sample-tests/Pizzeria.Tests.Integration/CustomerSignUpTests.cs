using FluentAssertions;
using Lewee.Playwright;
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
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        // Act - Register a new user via Keycloak
        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);

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

    [Fact]
    public async Task Should_NavigateToHomePage_When_UserSuccessfullyRegisters()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        // Act
        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);

        // Assert
        playwrightPage.Page.ShouldHaveBannerHeading();
    }
}
