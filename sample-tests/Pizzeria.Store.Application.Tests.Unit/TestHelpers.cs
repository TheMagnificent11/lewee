using Correlate;
using Lewee.Common;
using Lewee.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Tests.Unit;

internal static class TestHelpers
{
    public const string OwnerUserId = "owner-user-id";
    public const string OtherUserId = "other-user-id";

    public static NullLogger<T> Logger<T>() => NullLogger<T>.Instance;

    public static Pizza CreatePizza(string name = "Margherita", decimal price = 5.00m) =>
        Pizza.Create(name, "Tasty", price);

    public static Order CreateOrderWithPizza(out Pizza pizza, string userId = OwnerUserId)
    {
        pizza = CreatePizza();
        var order = Order.StartNewOrder(userId, Guid.NewGuid());
        order.AddPizza(pizza);
        order.DomainEvents.GetAndClear();
        return order;
    }

    public static Mock<IAuthenticatedUserService> AuthenticatedUser(string? userId)
    {
        var mock = new Mock<IAuthenticatedUserService>();
        mock.SetupGet(x => x.UserId).Returns(userId);
        return mock;
    }

    public static Mock<ICorrelationContextAccessor> CorrelationAccessor()
    {
        var mock = new Mock<ICorrelationContextAccessor>();
        mock.SetupGet(x => x.CorrelationContext)
            .Returns(new CorrelationContext { CorrelationId = Guid.NewGuid().ToString() });
        return mock;
    }

    public static Mock<IRepository<T>> Repository<T>()
        where T : AggregateRoot => new();
}
