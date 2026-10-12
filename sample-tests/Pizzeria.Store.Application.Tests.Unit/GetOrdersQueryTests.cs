using FluentAssertions;
using Lewee.Domain;
using Moq;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class GetOrdersQueryTests
{
    [Fact]
    public async Task Should_ReturnMappedOrders_When_ListingAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        order.Prepared(Guid.NewGuid());
        order.DomainEvents.GetAndClear();

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
}
