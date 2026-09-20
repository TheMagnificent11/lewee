using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class OrderFulfillmentTests
{
    [Fact]
    public async Task Should_MarkPickupOrderCompleted_When_PreparedAsync()
    {
        var order = CreatePreparedPickupOrder();
        var handler = CreatePickedUpHandler(order);

        var result = await handler.Handle(new MarkOrderPizzasPickedUpCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
        order.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Should_FailPickedUp_When_OrderNotPreparedAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        order.DomainEvents.GetAndClear();

        var handler = CreatePickedUpHandler(order);

        var result = await handler.Handle(new MarkOrderPizzasPickedUpCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task Should_MarkDeliveryOrderCompleted_When_PreparedAsync()
    {
        var order = CreatePreparedDeliveryOrder();
        var handler = CreateDeliveredHandler(order);

        var result = await handler.Handle(new MarkOrderPizzasDeliveredCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task Should_FailDelivered_When_OrderNotPreparedAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitDeliveryOrder("1 Pizza Street", Guid.NewGuid());
        order.DomainEvents.GetAndClear();

        var handler = CreateDeliveredHandler(order);

        var result = await handler.Handle(new MarkOrderPizzasDeliveredCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnMappedOrders_When_ListingAsync()
    {
        var order = CreatePreparedPickupOrder();

        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([order]);

        var handler = new GetOrdersQuery.Handler(orderRepository.Object);

        var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle().Which.Id.Should().Be(order.Id);
    }

    [Theory]
    [InlineData(OrderListFilter.Current)]
    [InlineData(OrderListFilter.Past)]
    public void Should_DeclareStoreStaffAndManagerRoles_ForOrdersQuery(OrderListFilter filter)
    {
        var query = new GetOrdersQuery(filter);

        query.TenantId.Should().Be(PizzaStore.Tenant.Id);
        query.Roles.Should().BeEquivalentTo(
            [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode]);
    }

    [Fact]
    public void Should_DeclareStoreStaffAndManagerRoles_ForFulfillmentCommands()
    {
        new MarkOrderPizzasPickedUpCommand(Guid.NewGuid()).Roles.Should().BeEquivalentTo(
            [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode]);
        new MarkOrderPizzasDeliveredCommand(Guid.NewGuid()).Roles.Should().BeEquivalentTo(
            [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode]);
        new StartMakingOrderCommand(Guid.NewGuid()).Roles.Should().BeEquivalentTo(
            [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode]);
        new MarkOrderPreparedCommand(Guid.NewGuid()).Roles.Should().BeEquivalentTo(
            [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode]);
    }

    [Fact]
    public async Task Should_TransitionThroughMakingAndPrepared_When_StaffProcessesPickupOrderAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        order.DomainEvents.GetAndClear();

        var startMaking = new StartMakingOrderCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<StartMakingOrderCommand.Handler>());
        var making = await startMaking.Handle(new StartMakingOrderCommand(order.Id), CancellationToken.None);

        making.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Making);

        var prepare = new MarkOrderPreparedCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<MarkOrderPreparedCommand.Handler>());
        var prepared = await prepare.Handle(new MarkOrderPreparedCommand(order.Id), CancellationToken.None);

        prepared.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.ReadyForPickup);
    }

    [Fact]
    public async Task Should_FailStartMaking_When_OrderNotReceivedAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);

        var handler = new StartMakingOrderCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<StartMakingOrderCommand.Handler>());

        var result = await handler.Handle(new StartMakingOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
    }

    private static Order CreatePreparedPickupOrder()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        order.Prepared(Guid.NewGuid());
        order.DomainEvents.GetAndClear();
        return order;
    }

    private static Order CreatePreparedDeliveryOrder()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitDeliveryOrder("1 Pizza Street", Guid.NewGuid());
        order.Prepared(Guid.NewGuid());
        order.DomainEvents.GetAndClear();
        return order;
    }

    private static MarkOrderPizzasPickedUpCommand.Handler CreatePickedUpHandler(Order order)
    {
        return new MarkOrderPizzasPickedUpCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<MarkOrderPizzasPickedUpCommand.Handler>());
    }

    private static MarkOrderPizzasDeliveredCommand.Handler CreateDeliveredHandler(Order order)
    {
        return new MarkOrderPizzasDeliveredCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<MarkOrderPizzasDeliveredCommand.Handler>());
    }

    private static Mock<IRepository<Order>> OrderRepository(Order order)
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        return orderRepository;
    }
}
