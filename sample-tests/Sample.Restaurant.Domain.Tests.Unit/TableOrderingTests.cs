using FluentAssertions;
using Lewee.Domain;
using Sample.Restaurant.Contracts;
using Xunit;

namespace Sample.Restaurant.Domain.Tests.Unit;

public class TableOrderingTests
{
    private readonly Table target;

    public TableOrderingTests()
    {
        this.target = new Table(Guid.NewGuid(), 17); // luka
    }

    [Fact]
    public void CannotOrderMenuItemWhenNotInUse()
    {
        var garlicBread = new MenuItem(Guid.NewGuid(), "Garlic Bread", 5, MenuItemType.Food);

        var action = () => this.target.OrderMenuItem(garlicBread, Guid.NewGuid());

        action.Should().Throw<DomainException>()
            .WithMessage(Table.ErrorMessages.CannotOrderIfTableNotInUse);
    }

    [Fact]
    public void CanOrderMenuItemWhenTableIsInUse()
    {
        var correlationId = Guid.NewGuid();
        var garlicBread = new MenuItem(Guid.NewGuid(), "Garlic Bread", 5, MenuItemType.Food);

        this.target.Use(Guid.NewGuid());
        this.target.OrderMenuItem(garlicBread, correlationId);

        this.target.CurrentOrder.Should().NotBeNull();
        this.target.CurrentOrder.Items.Should().NotBeNullOrEmpty();
        this.target.CurrentOrder.Items.Should().HaveCount(1);
        this.target.CurrentOrder.Total.Should().Be(garlicBread.Price);

        var orderedItem = this.target.CurrentOrder.Items.First();
        orderedItem.Should().NotBeNull();
        orderedItem.Id.Should().Be(Guid.Empty);
        orderedItem.OrderId.Should().Be(this.target.CurrentOrder.Id);
        orderedItem.MenuItemId.Should().Be(garlicBread.Id);
        orderedItem.Quantity.Should().Be(1);
        orderedItem.Price.Should().Be(garlicBread.Price);

        var domainEvents = orderedItem.DomainEvents.GetAndClear();
        domainEvents.Should().NotBeNullOrEmpty();
        domainEvents.Should().HaveCount(1);

        var orderItemEvent = domainEvents.First();
        orderItemEvent.Should().NotBeNull();
        orderItemEvent.Should().BeOfType<OrderItemAddedDomainEvent>();

        var orderItemAddedEvent = orderItemEvent as OrderItemAddedDomainEvent;
        orderItemAddedEvent.Should().NotBeNull();
        orderItemAddedEvent.CorrelationId.Should().Be(correlationId);
        orderItemAddedEvent.OrderItemId.Should().Be(orderedItem.Id);
        orderItemAddedEvent.MenuItemId.Should().Be(garlicBread.Id);
        orderItemAddedEvent.Price.Should().Be(garlicBread.Price);
        orderItemAddedEvent.OrderId.Should().Be(this.target.CurrentOrder.Id);
        orderItemAddedEvent.TableId.Should().Be(this.target.Id);
    }
}
