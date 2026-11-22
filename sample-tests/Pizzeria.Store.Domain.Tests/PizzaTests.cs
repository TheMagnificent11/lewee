using FluentAssertions;
using Xunit;

namespace Pizzeria.Store.Domain.Tests;

public sealed class PizzaTests
{
    [Fact]
    public void Create_WithValidDetails_CreatesPizza()
    {
        // Act
        var pizza = Pizza.Create("Hawaiian", "Ham and pineapple", 7.50m);

        // Assert
        pizza.Id.Should().NotBeEmpty();
        pizza.Name.Should().Be("Hawaiian");
        pizza.Description.Should().Be("Ham and pineapple");
        pizza.Price.Should().Be(7.50m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_Throws(string name)
    {
        var act = () => Pizza.Create(name, "description", 5.00m);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositivePrice_Throws(decimal price)
    {
        var act = () => Pizza.Create("Hawaiian", "description", price);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void UpdateDetails_WithValidDetails_UpdatesPizza()
    {
        // Arrange
        var pizza = Pizza.Create("Hawaiian", "Ham and pineapple", 7.50m);

        // Act
        pizza.UpdateDetails("Hawaiian Deluxe", "Ham, pineapple and extra cheese", 9.00m);

        // Assert
        pizza.Name.Should().Be("Hawaiian Deluxe");
        pizza.Description.Should().Be("Ham, pineapple and extra cheese");
        pizza.Price.Should().Be(9.00m);
    }

    [Fact]
    public void UpdateDetails_WithEmptyName_Throws()
    {
        var pizza = Pizza.Create("Hawaiian", "description", 7.50m);

        var act = () => pizza.UpdateDetails(string.Empty, "description", 5.00m);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateDetails_WithNonPositivePrice_Throws()
    {
        var pizza = Pizza.Create("Hawaiian", "description", 7.50m);

        var act = () => pizza.UpdateDetails("Hawaiian", "description", 0m);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
