using FluentAssertions;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Sample.Restaurant.Server.Tests.Integration.Tables;

public sealed class UseTableTests : TabeTestsBase
{
    public UseTableTests(RestaurantWebApplicationFactory factory)
        : base(factory)
    {
    }

    [BddfyFact]
    public void UseTable_ShouldSetupStoredQueryWhenTableIsntInUse()
    {
        var tableNumber = 4;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true, true))
            .Then(x => this.AnEmptyOrderIsCreatedForTable(tableNumber))
            .BDDfy();
    }

    [BddfyFact]
    public void UseTable_ShouldFailValidationWhenAttemptingToUseATableThatIsAlreadyInUse()
    {
        var tableNumber = 7;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true, true))
                .And(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, false, false))
            .Then(x => this.TheWaiterShouldntBeAbleToUseTheTableAsItIsAlreadyInUse())
            .BDDfy();
    }

    private async Task AnEmptyOrderIsCreatedForTable(int tableNumber)
    {
        var tableDetails = await this.HttpGet<TableDetailsDto>($"/api/v1/tables/{tableNumber}");

        this.ProblemDetails.Should().BeNull();

        tableDetails.Should().NotBeNull();
        tableDetails.TableNumber.Should().Be(tableNumber);

        var expectedMenuItems = MenuItem.DefaultData
            .OrderBy(x => x.ItemType)
            .ThenBy(x => x.Name)
            .ToArray();

        tableDetails.Items.Should().HaveCount(expectedMenuItems.Length);

        for (var i = 0; i < expectedMenuItems.Length; i++)
        {
            var expectedItem = expectedMenuItems[i];
            var actualItem = tableDetails.Items[i];

            actualItem.Should().NotBeNull();
            actualItem.Quantity.Should().Be(0);
            actualItem.MenuItem.Should().NotBeNull();
            actualItem.MenuItem.Id.Should().Be(expectedItem.Id);
            actualItem.MenuItem.Name.Should().Be(expectedItem.Name);
            actualItem.MenuItem.Price.Should().Be(expectedItem.Price);
            actualItem.MenuItem.PriceLabel.Should().Be($"{expectedItem.Price:0.00}");
        }
    }

    private void TheWaiterShouldntBeAbleToUseTheTableAsItIsAlreadyInUse()
    {
        this.ProblemDetails.Should().NotBeNull();
        this.ProblemDetails.Errors.Should().NotBeEmpty();
        this.ProblemDetails.Errors[string.Empty].Should().HaveCount(1);
        this.ProblemDetails.Errors[string.Empty][0].Should().Be(Table.ErrorMessages.TableInUse);
    }
}
