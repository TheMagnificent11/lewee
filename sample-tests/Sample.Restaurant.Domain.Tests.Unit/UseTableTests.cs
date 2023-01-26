using FluentAssertions;
using Lewee.Domain;
using Xunit;

namespace Sample.Restaurant.Domain.Tests.Unit;

public sealed class UseTableTests
{
    private readonly Table target;

    public UseTableTests()
    {
        this.target = new Table(Guid.NewGuid(), 3);
    }

    [Fact]
    public void CanUseWhenNotInUse()
    {
        var correlationId = Guid.NewGuid();
        var testStartTime = DateTime.UtcNow;

        this.target.Use(correlationId);
        var domainEventsRaised = this.target.DomainEvents.GetAndClear();

        this.target.IsInUse.Should().BeTrue();
        this.target.CurrentOrder.Should().NotBeNull();

        var orderDomainEvents = this.target.CurrentOrder.DomainEvents.GetAndClear();

        domainEventsRaised.Should().NotBeNullOrEmpty();
        domainEventsRaised.Should().HaveCount(1);

        var tableDomainEvent = domainEventsRaised.First();
        tableDomainEvent.Should().BeOfType<TableInUseDomainEvent>();

        var tableInUseEvent = tableDomainEvent as TableInUseDomainEvent;
        tableInUseEvent.Should().NotBeNull();
        tableInUseEvent.CorrelationId.Should().Be(correlationId);
        tableInUseEvent.TableId.Should().Be(this.target.Id);
        tableInUseEvent.TableNumber.Should().Be(this.target.TableNumber);

        orderDomainEvents.Should().NotBeNullOrEmpty();
        orderDomainEvents.Should().HaveCount(1);

        var orderDomainEvent = orderDomainEvents.First();
        orderDomainEvent.Should().BeOfType<OrderCreatedDomainEvent>();

        var orderCreatedEvent = orderDomainEvent as OrderCreatedDomainEvent;
        orderCreatedEvent.Should().NotBeNull();
        orderCreatedEvent.CorrelationId.Should().Be(correlationId);
        orderCreatedEvent.OrderId.Should().Be(this.target.CurrentOrder.Id);
        orderCreatedEvent.TableId.Should().Be(this.target.Id);
        orderCreatedEvent.TableNumber.Should().Be(this.target.TableNumber);
        orderCreatedEvent.CreatedDateTimeUtc.Should().BeAfter(testStartTime);
    }

    [Fact]
    public void CannotUseWhenInUse()
    {
        this.target.Use(Guid.NewGuid());

        var action = () => this.target.Use(Guid.NewGuid());

        action.Should().Throw<DomainException>()
            .WithMessage(Table.ErrorMessages.TableInUse);
    }
}
