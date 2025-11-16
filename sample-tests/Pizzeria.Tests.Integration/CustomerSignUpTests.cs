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
    public async Task Should_CreateCustomer_When_ValidExternalIdIsProvided()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";

        // Create user in Keycloak and get the user ID
        var keycloakUserId = await this.factory.CreateKeycloakUserAsync(username, password);
        var request = new CreateCustomerRequest { ExternalId = keycloakUserId };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var customer = await this.factory.GetLatestCustomerAsync();
        customer.Should().NotBeNull();
        customer.ExternalId.Should().Be(keycloakUserId);
        customer.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_RetrieveCustomerByExternalId_When_CustomerExists()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";

        // Create user in Keycloak and get the user ID
        var keycloakUserId = await this.factory.CreateKeycloakUserAsync(username, password);
        var request = new CreateCustomerRequest { ExternalId = keycloakUserId };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var customer = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId);
        customer.Should().NotBeNull();
        customer.ExternalId.Should().Be(keycloakUserId);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_ExternalIdIsEmpty()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var request = new CreateCustomerRequest { ExternalId = string.Empty };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_ExternalIdExceedsMaxLength()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var externalId = new string('a', 101); // Max length is 100
        var request = new CreateCustomerRequest { ExternalId = externalId };

        // Act
        using var response = await httpClient.PostAsJsonAsync(Endpoints.StoreApi.Customers, request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_CreateMultipleCustomers_When_DifferentExternalIdsProvided()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);
        var username1 = $"testuser-{Guid.NewGuid()}";
        var username2 = $"testuser-{Guid.NewGuid()}";
        var password = "TestPassword123!";

        // Create users in Keycloak and get their IDs
        var keycloakUserId1 = await this.factory.CreateKeycloakUserAsync(username1, password);
        var keycloakUserId2 = await this.factory.CreateKeycloakUserAsync(username2, password);

        // Act
        using var response1 = await httpClient.PostAsJsonAsync(
            Endpoints.StoreApi.Customers,
            new CreateCustomerRequest { ExternalId = keycloakUserId1 });
        await this.WaitForDomainEventsToBeDispatchedAsync();

        using var response2 = await httpClient.PostAsJsonAsync(
            Endpoints.StoreApi.Customers,
            new CreateCustomerRequest { ExternalId = keycloakUserId2 });
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert
        response1.EnsureSuccessStatusCode();
        response2.EnsureSuccessStatusCode();

        var customer1 = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId1);
        var customer2 = await this.factory.GetCustomerByExternalIdAsync(keycloakUserId2);

        customer1.Should().NotBeNull();
        customer2.Should().NotBeNull();
        customer1.Id.Should().NotBe(customer2.Id);
    }
}
