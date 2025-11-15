using FluentAssertions;
using Xunit;

namespace Pizzeria.Store.Domain.Tests;

public sealed class CustomerTests
{
    [Fact]
    public void Create_Should_Create_Valid_Customer()
    {
        // Arrange
        var externalId = Guid.NewGuid().ToString();
        var correlationId = Guid.NewGuid();

        // Act
        var customer = Customer.Create(externalId, correlationId);

        // Assert
        customer.Id.Should().NotBeEmpty();
        customer.ExternalId.Should().Be(externalId);
    }

    [Fact]
    public void Create_Should_Raise_CustomerCreatedEvent()
    {
        // Arrange
        var externalId = Guid.NewGuid().ToString();
        var correlationId = Guid.NewGuid();

        // Act
        var customer = Customer.Create(externalId, correlationId);
        var events = customer.DomainEvents.GetAndClear();

        // Assert
        events.Should().ContainSingle()
            .Which.Should().BeOfType<CustomerCreatedEvent>()
            .Which.Should().Match<CustomerCreatedEvent>(e =>
                e.CustomerId == customer.Id &&
                e.ExternalId == externalId &&
                e.CorrelationId == correlationId);
    }
}
