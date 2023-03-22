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
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true))
            .Then(x => this.AnEmptyOrderIsCreatedForTable(tableNumber));
    }

    [BddfyFact]
    public void UseTable_ShouldFailValidationWhenAttemptingToUseATableThatIsAlreadyInUse()
    {
        var tableNumber = 7;

        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, true))
                .And(x => this.TheWaiterSeatsACustomerAtTable(tableNumber, false))
            .Then(x => this.TheWaiterShouldntBeAbleToUseTheTableAsItIsAlreadyInUse());
    }

    private async Task GetTablesRequestIsExecuted()
    {
        this.Tables = await this.HttpGet<TableDto[]>("/tables");
    }

    private async Task TheWaiterSeatsACustomerAtTable(int tableNumber, bool expectSuccess)
    {
        await this.UseTable(tableNumber, expectSuccess);

        if (!expectSuccess)
        {
            return;
        }

        await this.WaitForDomainEventsToBeDispatched();
    }

    private async Task AnEmptyOrderIsCreatedForTable(int tableNumber)
    {
        var tableDetails = await this.HttpGet<TableDetailsDto>($"tables/{tableNumber}");

        // TODO: assert items
        this.ProblemDetails.Should().BeNull();
        tableDetails.Should().NotBeNull();
        tableDetails.TableNumber.Should().Be(tableNumber);
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
