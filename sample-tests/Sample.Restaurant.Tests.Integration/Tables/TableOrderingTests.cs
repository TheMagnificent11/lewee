using System.Net;
using FluentAssertions;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using TestStack.BDDfy;
using Xunit;

namespace Sample.Restaurant.Tests.Integration.Tables;

public sealed class TableOrderingTests : TableTestsBase
{
    public TableOrderingTests(
        RestaurantWebApplicationFactory webApplicationFactory,
        RestaurantDbContextFixture dbContextFixture)
        : base(webApplicationFactory, dbContextFixture)
    {
    }

    [Fact]
    public void Ordering_ShouldCorrectlyUpdateTheOrderWhenItemsAreAddedAndRemoved()
    {
        var tableNumber = 10;
        var expectedOrderItems = new MenuItem[]
        {
            Menu.Beer,
            Menu.Wine,
            Menu.GarlicBread,
            Menu.Pizza,
            Menu.IceCream,
            Menu.IceCream
        };

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true, false))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.Beer, true))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.Wine, true))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.GarlicBread, true))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.GarlicBread, true))
                .And(x => this.TheCustomerRemovesAnItemFromTheirOrder(tableNumber, Menu.GarlicBread, true))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.Pizza, true))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.IceCream, true))
                .And(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.IceCream, true))
                .And(x => this.TheCustomerHasFinishedOrdering())
            .Then(x => this.TheOrderForTheTableContainsTheCorrectItems(tableNumber, expectedOrderItems))
            .BDDfy();
    }

    [Fact]
    public void Ordering_ShouldFailToPlaceOrderWhenTableIsNotInUse()
    {
        var tableNumber = 3;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheCustomerOrdersAnItemOfTheMenu(tableNumber, Menu.Pasta, false))
            .Then(x => this.TheRestaurantManagerTellsTheCustomerThatTheyNeedToBeSeatedAtATableToOrderOffTheMenu())
            .BDDfy();
    }

    private async Task TheCustomerOrdersAnItemOfTheMenu(int tableNumber, MenuItem item, bool isSuccessExpected)
    {
        await this.OrderItem(tableNumber, item.Id, isSuccessExpected);
    }

    private async Task TheCustomerRemovesAnItemFromTheirOrder(int tableNumber, MenuItem item, bool isSuccessExpected)
    {
        await this.RemoveItem(tableNumber, item.Id, isSuccessExpected);
    }

    private async Task OrderItem(int tableNumber, Guid itemId, bool isSuccessExpected)
    {
        using (var response = await this.HttpRequest(HttpMethod.Put, $"/api/v1/tables/{tableNumber}/menu-items/{itemId}"))
        {
            if (isSuccessExpected)
            {
                response.EnsureSuccessStatusCode();
                return;
            }

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await this.SetProblemDetails(response);
        }
    }

    private async Task RemoveItem(int tableNumber, Guid itemId, bool isSuccessExpected)
    {
        using (var response = await this.HttpRequest(HttpMethod.Delete, $"/api/v1/tables/{tableNumber}/menu-items/{itemId}"))
        {
            if (isSuccessExpected)
            {
                response.EnsureSuccessStatusCode();
                return;
            }

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await this.SetProblemDetails(response);
        }
    }

    private async Task TheCustomerHasFinishedOrdering()
    {
        await this.WaitForDomainEventsToBeDispatched();
    }

    private async Task TheOrderForTheTableContainsTheCorrectItems(int tableNumber, MenuItem[] items)
    {
        var tableDetails = await this.HttpGet<TableDetailsDto>($"/api/v1/tables/{tableNumber}");

        var expectedItems = items
            .GroupBy(x => x.Id)
            .Select(x => new OrderedItem(x.First(), x.Count()))
            .ToDictionary(x => x.Item.Id, x => x);

        var orderedItems = tableDetails.Items
            .Where(x => x.Quantity > 0)
            .ToArray();

        orderedItems.Should().HaveCount(expectedItems.Count);

        foreach (var actualItem in orderedItems)
        {
            var expectedItem = expectedItems[actualItem.MenuItem.Id];
            actualItem.Quantity.Should().Be(expectedItem.Quantity);
            actualItem.MenuItem.Name.Should().Be(expectedItem.Item.Name);
            actualItem.MenuItem.Price.Should().Be(expectedItem.Item.Price);
        }
    }

    private void TheRestaurantManagerTellsTheCustomerThatTheyNeedToBeSeatedAtATableToOrderOffTheMenu()
    {
        this.ProblemDetails.Should().NotBeNull();
        this.ProblemDetails.Errors.Should().NotBeEmpty();
        this.ProblemDetails.Errors[string.Empty].Should().HaveCount(1);
        this.ProblemDetails.Errors[string.Empty][0].Should().Be(Table.ErrorMessages.CannotOrderIfTableNotInUse);
    }

    private readonly struct OrderedItem
    {
        public OrderedItem(MenuItem item, int quantity)
        {
            this.Item = item;
            this.Quantity = quantity;
        }

        public MenuItem Item { get; }
        public int Quantity { get; }
    }
}
