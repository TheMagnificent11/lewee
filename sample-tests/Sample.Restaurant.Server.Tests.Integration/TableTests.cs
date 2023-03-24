using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

public class TableTests : RestaurantTestsBase
{
    public TableTests(RestaurantWebApplicationFactory factory)
        : base(factory)
    {
    }

    private TableDto[] Tables { get; set; }
    private ValidationProblemDetails ProblemDetails { get; set; }

    [BddfyFact]
    public void GetTables_ShouldReturnTheCorrectTables()
    {
        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.GetTablesRequestIsExecuted())
            .Then(x => x.TheCorrectTablesAreReturned());
    }

    [BddfyFact]
    public void UseTable_ShouldSetupStoredQueryWhenTableIsntInUse()
    {
        var tableNumber = 4;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true, true))
            .Then(x => this.AnEmptyOrderIsCreatedForTable(tableNumber));
    }

    [BddfyFact]
    public void UseTable_ShouldFailValidationWhenAttemptingToUseATableThatIsAlreadyInUse()
    {
        var tableNumber = 7;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true, true))
                .And(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, false, false))
            .Then(x => this.TheWaiterShouldntBeAbleToUseTheTableAsItIsAlreadyInUse());
    }

    [BddfyFact]
    public void Ordering_ShouldCorrectlyUpdateTheOrderWhenItemsAreAddedAndRemoved()
    {
        var tableNumber = 10;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true, false))
                .And(x => this.TheCustomrOrdersAnItemOfTheMenu(tableNumber, Menu.Beer))
                .And(x => this.TheCustomrOrdersAnItemOfTheMenu(tableNumber, Menu.Wine))
                .And(x => this.TheCustomrOrdersAnItemOfTheMenu(tableNumber, Menu.GarlicBread))
                .And(x => this.TheCustomrOrdersAnItemOfTheMenu(tableNumber, Menu.GarlicBread))
                .And(x => this.TheCustomerRemovesAnItemFromTheirOrder(tableNumber, Menu.GarlicBread, true))
                .And(x => this.TheCustomrOrdersAnItemOfTheMenu(tableNumber, Menu.Pizza))
                .And(x => this.TheCustomrOrdersAnItemOfTheMenu(tableNumber, Menu.IceCream))
                .And(x => this.TheCustomerHasFinishedOrdering());
    }

    private async Task GetTablesRequestIsExecuted()
    {
        this.Tables = await this.HttpGet<TableDto[]>("/tables");
    }

    private async Task TheWaiterSeatsACustomerAtTable(int tableNumber, bool expectSuccess, bool dispatchEvents)
    {
        await this.UseTable(tableNumber, expectSuccess);

        if (!expectSuccess && !dispatchEvents)
        {
            return;
        }

        await this.WaitForDomainEventsToBeDispatched();
    }

    private async Task TheCustomrOrdersAnItemOfTheMenu(int tableNumber, MenuItem item)
    {
        await this.OrderItem(tableNumber, item.Id);
    }

    private async Task TheCustomerRemovesAnItemFromTheirOrder(int tableNumber, MenuItem item, bool expectSuccess)
    {
        await this.RemoveItem(tableNumber, item.Id, expectSuccess);

        if (!expectSuccess)
        {
            return;
        }
    }

    private async Task TheCustomerHasFinishedOrdering()
    {
        await this.WaitForDomainEventsToBeDispatched();
    }

    private async Task AnEmptyOrderIsCreatedForTable(int tableNumber)
    {
        var tableDetails = await this.HttpGet<TableDetailsDto>($"tables/{tableNumber}");

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

    private async Task TheOrderForTheTableContainsTheCorretItems(int tableNumber, MenuItem[] items)
    {
        var tableDetails = await this.HttpGet<TableDetailsDto>($"tables/{tableNumber}");
        var orders = items.GroupBy(x => x.Id);
    }

    private void TheWaiterShouldntBeAbleToUseTheTableAsItIsAlreadyInUse()
    {
        this.ProblemDetails.Should().NotBeNull();
        this.ProblemDetails.Errors.Should().NotBeEmpty();
        this.ProblemDetails.Errors[string.Empty].Should().HaveCount(1);
        this.ProblemDetails.Errors[string.Empty][0].Should().Be(Table.ErrorMessages.TableInUse);
    }

    private async Task UseTable(int tableNumber, bool isSuccessExpected)
    {
        using (var response = await this.HttpRequest(HttpMethod.Post, $"/tables/{tableNumber}"))
        {
            if (isSuccessExpected)
            {
                response.EnsureSuccessStatusCode();
                return;
            }

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            this.ProblemDetails = await this.DeserializeResponse<ValidationProblemDetails>(response);
        }
    }

    private async Task OrderItem(int tableNumber, Guid itemId)
    {
        using (var response = await this.HttpRequest(HttpMethod.Put, $"/tables/{tableNumber}/menu-items/{itemId}"))
        {
            response.EnsureSuccessStatusCode();
        }
    }

    private async Task RemoveItem(int tableNumber, Guid itemId, bool isSuccessExpected)
    {
        using (var response = await this.HttpRequest(HttpMethod.Delete, $"/tables/{tableNumber}/menu-items/{itemId}"))
        {
            if (isSuccessExpected)
            {
                response.EnsureSuccessStatusCode();
                return;
            }

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            this.ProblemDetails = await this.DeserializeResponse<ValidationProblemDetails>(response);
        }
    }

    private void TheCorrectTablesAreReturned()
    {
        this.Tables.Should().NotBeNull();
        this.Tables.Should().HaveCount(10);

        for (var i = 1; i <= 10; i++)
        {
            var table = this.Tables[i];
            table.Should().NotBeNull();
            table.Id.Should().NotBeEmpty();
            table.TableNumber.Should().Be(i);
            table.Status.Should().Be(TableDto.Statuses.Available);
        }
    }
}
