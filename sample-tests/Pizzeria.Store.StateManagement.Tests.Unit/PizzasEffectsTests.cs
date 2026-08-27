using Correlate;
using Fluxor;
using Lewee.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class PizzasEffectsTests
{
    private readonly Mock<IBffApiClient> bffApiClientMock = new();
    private readonly Mock<IDispatcher> dispatcherMock = new();
    private readonly PizzasEffects effects;

    public PizzasEffectsTests()
    {
        this.effects = new PizzasEffects(
            this.bffApiClientMock.Object,
            Mock.Of<IState<PizzasState>>(),
            Mock.Of<ICorrelationContextAccessor>(),
            Mock.Of<ILogger<PizzasEffects>>());
    }

    [Fact]
    public async Task OnQueryAsync_Success_DispatchesSuccessActionAsync()
    {
        // Arrange
        var pizzas = new PizzaDto[]
        {
            new(Guid.NewGuid(), "Margherita", "Classic pizza", 9.99m),
            new(Guid.NewGuid(), "Pepperoni", "Spicy pizza", 11.99m),
        };

        this.bffApiClientMock
            .Setup(x => x.GetPizzasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizzas);

        var action = new LoadPizzasAction { CorrelationId = Guid.NewGuid() };

        // Act
        await this.effects.OnQueryAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<LoadPizzasSuccessAction>(a => a.Data.SequenceEqual(pizzas))),
            Times.Once);
    }

    [Fact]
    public async Task OnQueryAsync_Exception_DispatchesFailureActionAsync()
    {
        // Arrange
        var exceptionMessage = "Something went wrong";
        this.bffApiClientMock
            .Setup(x => x.GetPizzasAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        var action = new LoadPizzasAction { CorrelationId = Guid.NewGuid() };

        // Act
        await this.effects.OnQueryAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<LoadPizzasFailureAction>(a => a.ErrorMessage.Contains(exceptionMessage))),
            Times.Once);
    }
}
