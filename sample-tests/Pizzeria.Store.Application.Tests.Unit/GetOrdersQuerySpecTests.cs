using FluentAssertions;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class GetOrdersQuerySpecTests
{
    [Fact]
    public void Should_MatchSubmittedNotCompletedOrders_When_CurrentFilter()
    {
        var predicate = BuildPredicate(OrderListFilter.Current);

        predicate(InProgressOrder()).Should().BeFalse();
        predicate(SubmittedOrder()).Should().BeTrue();
        predicate(CompletedOrder()).Should().BeFalse();
    }

    [Fact]
    public void Should_MatchCompletedOrders_When_PastFilter()
    {
        var predicate = BuildPredicate(OrderListFilter.Past);

        predicate(InProgressOrder()).Should().BeFalse();
        predicate(SubmittedOrder()).Should().BeFalse();
        predicate(CompletedOrder()).Should().BeTrue();
    }

    private static Func<Order, bool> BuildPredicate(OrderListFilter filter)
    {
        var spec = new GetOrdersQuerySpec(filter);
        return spec.WhereExpressions.Single().Compile();
    }

    private static Order InProgressOrder()
    {
        return TestHelpers.CreateOrderWithPizza(out _);
    }

    private static Order SubmittedOrder()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        return order;
    }

    private static Order CompletedOrder()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        order.Prepared(Guid.NewGuid());
        order.PickedUp(Guid.NewGuid());
        return order;
    }
}
