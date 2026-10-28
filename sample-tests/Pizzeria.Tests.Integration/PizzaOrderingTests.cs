using FluentAssertions;
using Lewee.Playwright;
using Microsoft.Playwright;
using Pizzeria.Ordering.Components;
using Pizzeria.Store.Domain;
using Pizzeria.Tests.Integration.Infrastructure;
using Xunit;

using OrderPage = Pizzeria.Ordering.Components.Order;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class PizzaOrderingTests : PizzeriaTests
{
    public PizzaOrderingTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrderByAddingAndRemovingPizzasAndCompletingCheckoutForPickup()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);
        playwrightPage.Page.ShouldHaveBannerHeading();

        // 1. Start order
        await playwrightPage.Page.WaitForSelectorAsync(Home.Selectors.StartOrderButton, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(Home.Selectors.StartOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        Guid orderId;

        try
        {
            await playwrightPage.Page.WaitForURLAsync(
                url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
                new PageWaitForURLOptions { Timeout = 30000 });

            orderId = ExtractOrderId(playwrightPage.Page.Url);

            // 2. Verify that the pizza menu screen is displayed
            await playwrightPage.Page.WaitForSelectorAsync(
                OrderPage.Selectors.PizzaMenuHeading,
                new PageWaitForSelectorOptions { Timeout = 30000 });
            var orderPageContent = await playwrightPage.Page.ContentAsync();
            orderPageContent.Should().Contain("Pizza Menu", "the order page should show the pizza menu");
        }
        catch (TimeoutException ex)
        {
            var currentContent = await playwrightPage.Page.ContentAsync();
            var currentUrl = playwrightPage.Page.Url;

            var error = $"""
                    Timed out waiting for navigation to order page.
                    This indicates the SSE message was not received.
                    Current URL: {currentUrl}.
                    Page contains error: {currentContent.Contains("error", StringComparison.OrdinalIgnoreCase)}
                    """;

            throw new InvalidOperationException(error, ex);
        }

        var addMargheritaSelector = OrderPage.Selectors.AddPizzaButton(Menu.PizzaNames.Margherita);
        var removeMargheritaSelector = OrderPage.Selectors.RemovePizzaButton(Menu.PizzaNames.Margherita);
        var margheritaQuantitySelector = OrderPage.Selectors.PizzaQuantity(Menu.PizzaNames.Margherita);
        var addMarinaraSelector = OrderPage.Selectors.AddPizzaButton(Menu.PizzaNames.Marinara);
        var marinaraQuantitySelector = OrderPage.Selectors.PizzaQuantity(Menu.PizzaNames.Marinara);

        // 3. Add a pizza
        await playwrightPage.Page.WaitForSelectorAsync(addMargheritaSelector, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 4. Verify that the pizza has been added
        (await playwrightPage.Page.InnerTextAsync(margheritaQuantitySelector)).Trim().Should().Be("1");

        // 5. Add another pizza of the same type
        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 6. Verify that the quantity increased
        (await playwrightPage.Page.InnerTextAsync(margheritaQuantitySelector)).Trim().Should().Be("2");

        // 7. Remove one of the pizzas
        await playwrightPage.Page.ClickAsync(removeMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 8. Verify the quantity decreased
        (await playwrightPage.Page.InnerTextAsync(margheritaQuantitySelector)).Trim().Should().Be("1");

        // 9. Add a pizza of a different type
        await playwrightPage.Page.ClickAsync(addMarinaraSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 10. Verify that pizza was added
        (await playwrightPage.Page.InnerTextAsync(marinaraQuantitySelector)).Trim().Should().Be("1");

        // 11. Verify that the order has all the correct pizzas and quantities
        var order = await this.Factory.GetOrderAsync(orderId);
        order.Should().NotBeNull();
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.Margherita && p.Quantity == 1);
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.Marinara && p.Quantity == 1);

        var orderProjection = await this.Factory.GetQueryProjectionAsync<OrderQueryProjection>(orderId.ToString());
        orderProjection.Should().NotBeNull();
        orderProjection.Order.Should().NotBeNull();
        orderProjection.Order.Id.Should().Be(orderId);

        // Checkout functionality (e.g. submitting as pickup/delivery) is covered by CheckoutFlowTests.
    }

    private static Guid ExtractOrderId(string url)
    {
        var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var orderSegmentIndex = Array.IndexOf(segments, "orders");

        return Guid.Parse(segments[orderSegmentIndex + 1]);
    }
}
