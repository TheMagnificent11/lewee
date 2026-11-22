using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Common;
using Pizzeria.Store.Application.Pizzas;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class MenuManagementTests
{
    [Fact]
    public async Task Should_AddAvailablePizza_When_AddCommandHandledAsync()
    {
        Pizza? added = null;
        var repository = TestHelpers.Repository<Pizza>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<Pizza>(), It.IsAny<CancellationToken>()))
            .Callback<Pizza, CancellationToken>((pizza, _) => added = pizza);

        var handler = new AddPizzaCommand.Handler(
            repository.Object,
            TestHelpers.Logger<AddPizzaCommand.Handler>());

        var result = await handler.Handle(
            new AddPizzaCommand("Margherita", "Classic", 9.99m),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        added.Should().NotBeNull();
        added.Name.Should().Be("Margherita");
        added.IsAvailable.Should().BeTrue();
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_UpdatePizzaDetails_When_EditCommandHandledAsync()
    {
        var pizza = TestHelpers.CreatePizza();
        var handler = new EditPizzaCommand.Handler(
            PizzaRepository(pizza).Object,
            TestHelpers.Logger<EditPizzaCommand.Handler>());

        var result = await handler.Handle(
            new EditPizzaCommand(pizza.Id, "Pepperoni", "Spicy", 12.50m),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        pizza.Name.Should().Be("Pepperoni");
        pizza.Description.Should().Be("Spicy");
        pizza.Price.Should().Be(12.50m);
    }

    [Fact]
    public async Task Should_FailEdit_When_PizzaNotFoundAsync()
    {
        var repository = TestHelpers.Repository<Pizza>();
        repository
            .Setup(x => x.RetrieveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pizza?)null);

        var handler = new EditPizzaCommand.Handler(
            repository.Object,
            TestHelpers.Logger<EditPizzaCommand.Handler>());

        var result = await handler.Handle(
            new EditPizzaCommand(Guid.NewGuid(), "Pepperoni", "Spicy", 12.50m),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Should_SoftDeletePizza_When_RemoveCommandHandledAsync()
    {
        var pizza = TestHelpers.CreatePizza();
        var handler = new RemovePizzaCommand.Handler(
            PizzaRepository(pizza).Object,
            TestHelpers.Logger<RemovePizzaCommand.Handler>());

        var result = await handler.Handle(new RemovePizzaCommand(pizza.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        pizza.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Should_FailRemove_When_PizzaNotFoundAsync()
    {
        var repository = TestHelpers.Repository<Pizza>();
        repository
            .Setup(x => x.RetrieveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pizza?)null);

        var handler = new RemovePizzaCommand.Handler(
            repository.Object,
            TestHelpers.Logger<RemovePizzaCommand.Handler>());

        var result = await handler.Handle(new RemovePizzaCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public void Should_DeclareStoreManagerRole_ForMenuCommands()
    {
        new AddPizzaCommand("A", null, 1m).TenantId.Should().Be(PizzaStore.Tenant.Id);
        new AddPizzaCommand("A", null, 1m).Roles.Should().BeEquivalentTo([PizzaStore.Roles.StoreManagerCode]);
        new EditPizzaCommand(Guid.NewGuid(), "A", null, 1m).Roles
            .Should().BeEquivalentTo([PizzaStore.Roles.StoreManagerCode]);
        new RemovePizzaCommand(Guid.NewGuid()).Roles
            .Should().BeEquivalentTo([PizzaStore.Roles.StoreManagerCode]);
    }

    private static Mock<IRepository<Pizza>> PizzaRepository(Pizza pizza)
    {
        var repository = TestHelpers.Repository<Pizza>();
        repository
            .Setup(x => x.RetrieveByIdAsync(pizza.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizza);
        return repository;
    }
}
