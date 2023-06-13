using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Restaurant.Server.Tests.Integration.Tables;

public abstract class TabeTestsBase : RestaurantTestsBase
{
    protected TabeTestsBase(RestaurantWebApplicationFactory factory)
        : base(factory)
    {
    }

    protected ValidationProblemDetails ProblemDetails { get; private set; }

    protected async Task SetProblemDetails(HttpResponseMessage response)
    {
        this.ProblemDetails = await this.DeserializeResponse<ValidationProblemDetails>(response);
    }

    protected async Task TheWaiterSeatsACustomerAtTable(int tableNumber, bool isSuccessExpected, bool dispatchEvents)
    {
        await this.UseTable(tableNumber, isSuccessExpected);

        if (!isSuccessExpected && !dispatchEvents)
        {
            return;
        }

        await this.WaitForDomainEventsToBeDispatched();
    }

    private async Task UseTable(int tableNumber, bool isSuccessExpected)
    {
        using (var response = await this.HttpRequest(HttpMethod.Put, $"/api/v1/tables/{tableNumber}"))
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
}
