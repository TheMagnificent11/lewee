using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Sample.Restaurant.Application;
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
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber));
    }

    private async Task GetTablesRequestIsExecuted()
    {
        this.Tables = await this.HttpGet<TableDto[]>("/tables");
    }

    private async Task TheWaiterSeatsACustomerAtTable(int tableNumber)
    {
        await this.UseTable(4, true);
        await this.WaitForDomainEventsToBeDispatched();
    }

    private async Task AnEmptyOrderIsCreatedForTable(int tableNumber)
    {

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
