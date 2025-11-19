using System.Net.Http.Json;
using FluentAssertions;
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
}
