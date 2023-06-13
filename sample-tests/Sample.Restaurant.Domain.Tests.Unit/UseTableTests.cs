using FluentAssertions;
using Lewee.Domain;
using NUnit.Framework;

namespace Sample.Restaurant.Domain.Tests.Unit;

[TestFixture]
public sealed class UseTableTests
{
    private Table target;

    [SetUp]
    public void Setup()
    {
        this.target = new Table(Guid.NewGuid(), 3);
    }

    [Test]
    public void CanUseWhenNotInUse()
    {
        var correlationId = Guid.NewGuid();

        this.target.Use(correlationId);
        var domainEventsRaised = this.target.DomainEvents.GetAndClear();

        this.target.IsInUse.Should().BeTrue();
        this.target.CurrentOrder.Should().NotBeNull();

        domainEventsRaised.Should().NotBeNullOrEmpty();
        domainEventsRaised.Should().HaveCount(1);

        var tableDomainEvent = domainEventsRaised.First();
        tableDomainEvent.Should().BeOfType<TableInUseDomainEvent>();

        var tableInUseEvent = tableDomainEvent as TableInUseDomainEvent;
        tableInUseEvent.Should().NotBeNull();
        tableInUseEvent.CorrelationId.Should().Be(correlationId);
        tableInUseEvent.TableId.Should().Be(this.target.Id);
        tableInUseEvent.TableNumber.Should().Be(this.target.TableNumber);
    }

    [Test]
    public void CannotUseWhenInUse()
    {
        this.target.Use(Guid.NewGuid());

        var action = () => this.target.Use(Guid.NewGuid());

        action.Should().Throw<DomainException>()
            .WithMessage(Table.ErrorMessages.TableInUse);
    }
}
