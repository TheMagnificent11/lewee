using FluentAssertions;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public static class PizzaStoreApplicationConfigurationTests
{
    [Fact]
    public static void AddPizzaStoreApplication_ShouldRegisterApplicationServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFakeLogging();
        services.AddPizzaStoreApplication();
        services.AddSingleton(_ => Mock.Of<IRepository<Pizza>>());
        services.AddSingleton(_ => Mock.Of<IRepository<Order>>());
        services.AddSingleton(_ => Mock.Of<IQueryProjectionService>());

        using var serviceProvider = services.BuildServiceProvider();

        // Assert
        var addPizzaToOrderHandler = serviceProvider.GetService<IRequestHandler<AddPizzaToOrderCommand, CommandResult>>();
        addPizzaToOrderHandler.Should().NotBeNull();

        var orderingHandler = serviceProvider.GetService<INotificationHandler<OrderStartedEvent>>();
        orderingHandler.Should().NotBeNull();
    }
}
