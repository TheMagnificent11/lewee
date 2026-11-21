using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Configuration.Tests.Unit;

public sealed class StoreDbContextQueryFilterTests
{
    [Fact]
    public void Should_TranslateOrderPizzaQueries_When_UsingPostgreSql()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseNpgsql("Host=localhost;Database=pizzeria")
            .ConfigureWarnings(warnings => warnings.Throw(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning))
            .Options;

        using var dbContext = new StoreDbContext(options);
        dbContext.Set<OrderPizza>().ToQueryString().Should().NotBeNullOrEmpty();
        dbContext.Set<OrderPizza>().Include(x => x.Order).Include(x => x.Pizza)
            .ToQueryString().Should().NotBeNullOrEmpty();
        dbContext.Orders.Include(x => x.Pizzas).ThenInclude(x => x.Pizza)
            .ToQueryString().Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, false)]
    public async Task Should_FilterOrderPizzas_When_TheirRequiredEntitiesAreDeletedAsync(
        bool deleteOrder,
        bool deletePizza,
        bool expectedVisible)
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Throw(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning))
            .Options;

        await using var dbContext = new StoreDbContext(options);
        var order = Order.StartNewOrder("user", Guid.NewGuid());
        var pizza = Pizza.Create("Pizza", "Description", 10);
        order.AddPizza(pizza, Guid.NewGuid());
        if (deleteOrder)
        {
            order.Delete();
        }

        if (deletePizza)
        {
            pizza.Delete();
        }

        dbContext.AddRange(order, pizza);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var orderPizzas = dbContext.Set<OrderPizza>();
        (await orderPizzas.IgnoreQueryFilters().CountAsync()).Should().Be(1);
        (await orderPizzas.CountAsync()).Should().Be(expectedVisible ? 1 : 0);
        (await orderPizzas.Include(x => x.Order).Include(x => x.Pizza).CountAsync())
            .Should().Be(expectedVisible ? 1 : 0);
    }
}
