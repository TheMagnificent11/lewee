using FluentAssertions;
using Sample.Restaurant.Application;
using TestStack.BDDfy;
using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration.Tables;

public sealed class GetTablesTests : TableTestsBase
{
    public GetTablesTests(
        RestaurantWebApplicationFactory webApplicationFactory,
        RestaurantDbContextFixture dbContextFixture)
        : base(webApplicationFactory, dbContextFixture)
    {
    }

    private TableDto[] Tables { get; set; }

    [Fact]
    public void GetTables_ShouldReturnTheCorrectTables()
    {
        this.Given(x => this.AnEmptyRestaurant())
            .When(x => this.GetTablesRequestIsExecuted())
            .Then(x => x.TheCorrectTablesAreReturned())
            .BDDfy();
    }

    private async Task GetTablesRequestIsExecuted()
    {
        this.Tables = await this.HttpGet<TableDto[]>("/api/v1/tables");
    }

    private void TheCorrectTablesAreReturned()
    {
        this.Tables.Should().NotBeNull();
        this.Tables.Should().HaveCount(10);

        for (var i = 0; i < 10; i++)
        {
            var table = this.Tables[i];
            table.Should().NotBeNull();
            table.Id.Should().NotBeEmpty();
            table.TableNumber.Should().Be(i + 1);
            table.Status.Should().Be(TableDto.Statuses.Available);
        }
    }
}
