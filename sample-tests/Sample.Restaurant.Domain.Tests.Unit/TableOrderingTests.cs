using FluentAssertions;
using Lewee.Domain;
using Xunit;

namespace Sample.Restaurant.Domain.Tests.Unit;

public class TableOrderingTests
{
    private readonly Table target;

    public TableOrderingTests()
    {
        this.target = new Table(Guid.NewGuid(), 17);
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
        var pizza = new MenuItem(Guid.NewGuid(), "Pizza", 20, MenuItemType.Food);

        this.target.Use(Guid.NewGuid());
        this.target.DomainEvents.GetAndClear(); // reset domain events before performing action under test

        this.target.OrderMenuItem(pizza, correlationId);

        this.target.CurrentOrder.Should().NotBeNull();
        this.target.CurrentOrder.Items.Should().NotBeNullOrEmpty();
        this.target.CurrentOrder.Items.Should().HaveCount(1);
        this.target.CurrentOrder.Total.Should().Be(pizza.Price);

        var orderedItem = this.target.CurrentOrder.Items.First();
        orderedItem.Should().NotBeNull();
        orderedItem.Id.Should().Be(Guid.Empty);
        orderedItem.OrderId.Should().Be(this.target.CurrentOrder.Id);
        orderedItem.MenuItemId.Should().Be(pizza.Id);
        orderedItem.Quantity.Should().Be(1);
        orderedItem.Price.Should().Be(pizza.Price);

        var domainEvents = this.target.DomainEvents.GetAndClear();
        domainEvents.Should().NotBeNullOrEmpty();
        domainEvents.Should().HaveCount(1);

        var orderItemEvent = domainEvents.First();
        orderItemEvent.Should().NotBeNull();
        orderItemEvent.Should().BeOfType<OrderItemAddedDomainEvent>();

        var orderItemAddedEvent = orderItemEvent as OrderItemAddedDomainEvent;
        orderItemAddedEvent.Should().NotBeNull();
        orderItemAddedEvent.CorrelationId.Should().Be(correlationId);
        orderItemAddedEvent.MenuItemId.Should().Be(pizza.Id);
        orderItemAddedEvent.Price.Should().Be(pizza.Price);
        orderItemAddedEvent.OrderId.Should().Be(this.target.CurrentOrder.Id);
        orderItemAddedEvent.TableId.Should().Be(this.target.Id);
    }

    [Fact]
    public void CannotRemoveMenuItemWhenNotInUse()
    {
        var redWine = new MenuItem(Guid.NewGuid(), "House Red Wine (Glass)", 10, MenuItemType.Drink);

        var action = () => this.target.RemovedMenuItem(redWine, Guid.NewGuid());

        action.Should().Throw<DomainException>()
            .WithMessage(Table.ErrorMessages.CannotOrderIfTableNotInUse);
    }

    [Fact]
    public void NothingHappensWhenRemovingMenuItemNotAddedToOrder()
    {
        var juice = new MenuItem(Guid.NewGuid(), "Juice", 3, MenuItemType.Drink);
        var softDrink = new MenuItem(Guid.NewGuid(), "Soft Drink", 2.5m, MenuItemType.Drink);

        this.target.Use(Guid.NewGuid());
        this.target.OrderMenuItem(juice, Guid.NewGuid());

        var originalItemsCount = this.target.CurrentOrder.Items.Count;

        var action = () => this.target.RemovedMenuItem(softDrink, Guid.NewGuid());

        action.Should().NotThrow<DomainException>();

        this.target.CurrentOrder.Items.Should().HaveCount(originalItemsCount);
    }

    [Fact]
    public void RemovesCorrectItemWhenRemovingItemPreviouslyAddedToOrder()
    {
        var correlationId = Guid.NewGuid();

        var beer = new MenuItem(Guid.NewGuid(), "Beer", 8, MenuItemType.Drink);
        var redWine = new MenuItem(Guid.NewGuid(), "House Red Wine (Glass)", 10, MenuItemType.Drink);
        var garlicBread = new MenuItem(Guid.NewGuid(), "Garlic Bread", 5, MenuItemType.Food);
        var pasta = new MenuItem(Guid.NewGuid(), "Pasta", 17.5m, MenuItemType.Food);
        var pizza = new MenuItem(Guid.NewGuid(), "Pizza", 20, MenuItemType.Food);
        var icecream = new MenuItem(Guid.NewGuid(), "Icecream", 7.5m, MenuItemType.Food);

        this.target.Use(Guid.NewGuid());
        this.target.OrderMenuItem(beer, Guid.NewGuid());
        this.target.OrderMenuItem(redWine, Guid.NewGuid());
        this.target.OrderMenuItem(garlicBread, Guid.NewGuid());
        this.target.OrderMenuItem(garlicBread, Guid.NewGuid());
        this.target.OrderMenuItem(pasta, Guid.NewGuid());
        this.target.OrderMenuItem(pizza, Guid.NewGuid());
        this.target.OrderMenuItem(icecream, Guid.NewGuid());

        this.target.DomainEvents.GetAndClear(); // reset domain events before performing action under test

        this.target.RemovedMenuItem(garlicBread, correlationId);

        this.target.CurrentOrder.Items.Should().HaveCount(6);

        var garlicBreadOrderItem = this.target.CurrentOrder.Items.FirstOrDefault(x => x.MenuItemId == garlicBread.Id);
        garlicBreadOrderItem.Should().NotBeNull();
        garlicBreadOrderItem.Quantity.Should().Be(1);

        var domainEvents = this.target.DomainEvents.GetAndClear();
        domainEvents.Should().HaveCount(1);
        domainEvents[0].Should().BeOfType<OrderItemRemovedDomainEvent>();

        var itemRemovedEvent = domainEvents[0] as OrderItemRemovedDomainEvent;
        itemRemovedEvent.Should().NotBeNull();
        itemRemovedEvent.CorrelationId.Should().Be(correlationId);
        itemRemovedEvent.MenuItemId.Should().Be(garlicBread.Id);
        itemRemovedEvent.Price.Should().Be(garlicBread.Price);
        itemRemovedEvent.OrderId.Should().Be(this.target.CurrentOrder.Id);
        itemRemovedEvent.TableId.Should().Be(this.target.Id);
    }
}
