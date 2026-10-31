using FluentAssertions;
using Lewee.Playwright;
using Microsoft.Playwright;
using Pizzeria.Ordering.Components;
using Pizzeria.Store.Domain;
using Pizzeria.Tests.E2E.Infrastructure;
using Xunit;

using static Microsoft.Playwright.Assertions;
using OrderPage = Pizzeria.Ordering.Components.Order;

namespace Pizzeria.Tests.E2E;

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

        var orderId = await WaitForOrderPageAsync(playwrightPage.Page);

        // 2. Verify that the pizza menu screen is displayed
        var orderPageContent = await playwrightPage.Page.ContentAsync();
        orderPageContent.Should().Contain("Pizza Menu", "the order page should show the pizza menu");

        var addMargheritaSelector = OrderPage.Selectors.AddPizzaButton(Menu.PizzaNames.Margherita);
        var removeMargheritaSelector = OrderPage.Selectors.RemovePizzaButton(Menu.PizzaNames.Margherita);
        var margheritaQuantityLocator = playwrightPage.Page.Locator(OrderPage.Selectors.PizzaQuantity(Menu.PizzaNames.Margherita));
        var addMarinaraSelector = OrderPage.Selectors.AddPizzaButton(Menu.PizzaNames.Marinara);
        var marinaraQuantityLocator = playwrightPage.Page.Locator(OrderPage.Selectors.PizzaQuantity(Menu.PizzaNames.Marinara));

        // 3. Add a pizza
        await playwrightPage.Page.WaitForSelectorAsync(addMargheritaSelector, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 4. Verify that the pizza has been added
        await Expect(margheritaQuantityLocator).ToHaveTextAsync("1");

        // 5. Add another pizza of the same type
        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 6. Verify that the quantity increased
        await Expect(margheritaQuantityLocator).ToHaveTextAsync("2");

        // 7. Remove one of the pizzas
        await playwrightPage.Page.ClickAsync(removeMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 8. Verify the quantity decreased
        await Expect(margheritaQuantityLocator).ToHaveTextAsync("1");

        // 9. Add a pizza of a different type
        await playwrightPage.Page.ClickAsync(addMarinaraSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 10. Verify that pizza was added
        await Expect(marinaraQuantityLocator).ToHaveTextAsync("1");

        // 11. Verify that the order has all the correct pizzas and quantities
        var order = await this.Factory.GetOrderAsync(orderId);
        order.Should().NotBeNull();
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.Margherita && p.Quantity == 1);
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.Marinara && p.Quantity == 1);

        var orderProjection = await this.Factory.GetQueryProjectionAsync<OrderQueryProjection>(orderId.ToString());
        orderProjection.Should().NotBeNull();
        orderProjection.Order.Should().NotBeNull();
        orderProjection.Order.Id.Should().Be(orderId);

        // 12. Checkout as pickup
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.CheckoutButton);
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.SubmitOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 13. Verify that the order status is now shown and the order was submitted as a pickup order
        await playwrightPage.Page.WaitForSelectorAsync(
            OrderPage.Selectors.OrderStatus,
            new PageWaitForSelectorOptions { Timeout = 30000 });

        var completedOrder = await this.Factory.GetOrderAsync(orderId);
        completedOrder.Should().NotBeNull();
        completedOrder.IsDeliveryOrder.Should().BeFalse();
        completedOrder.SubmittedDateTime.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateOrderByAddingPizzasAndCompletingDeliveryCheckout()
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

        var orderId = await WaitForOrderPageAsync(playwrightPage.Page);

        // 2. Add a Quattro Stagioni pizza
        var addQuattroStagioniSelector = OrderPage.Selectors.AddPizzaButton(Menu.PizzaNames.QuattroStagioni);
        var quattroStagioniQuantityLocator = playwrightPage.Page.Locator(OrderPage.Selectors.PizzaQuantity(Menu.PizzaNames.QuattroStagioni));
        var addCarbonaraSelector = OrderPage.Selectors.AddPizzaButton(Menu.PizzaNames.Carbonara);
        var carbonaraQuantityLocator = playwrightPage.Page.Locator(OrderPage.Selectors.PizzaQuantity(Menu.PizzaNames.Carbonara));

        await playwrightPage.Page.WaitForSelectorAsync(addQuattroStagioniSelector, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(addQuattroStagioniSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await Expect(quattroStagioniQuantityLocator).ToHaveTextAsync("1");

        // 3. Add a pizza of a different type
        await playwrightPage.Page.ClickAsync(addCarbonaraSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await Expect(carbonaraQuantityLocator).ToHaveTextAsync("1");

        // 4. Checkout as delivery
        const string deliveryAddress = "456 Pizza Avenue, Pizzaville";

        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.CheckoutButton);
        await playwrightPage.Page.ClickAsync("text=Delivery");
        await playwrightPage.Page.FillAsync(OrderPage.Selectors.DeliveryAddressInput, deliveryAddress);
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.SubmitOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // 5. Verify that the order status is now shown and the order was submitted as a delivery order
        await playwrightPage.Page.WaitForSelectorAsync(
            OrderPage.Selectors.OrderStatus,
            new PageWaitForSelectorOptions { Timeout = 30000 });

        var order = await this.Factory.GetOrderAsync(orderId);
        order.Should().NotBeNull();
        order.IsDeliveryOrder.Should().BeTrue();
        order.DeliveryAddress.Should().Be(deliveryAddress);
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.QuattroStagioni && p.Quantity == 1);
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.Carbonara && p.Quantity == 1);
    }

    private static async Task<Guid> WaitForOrderPageAsync(IPage page)
    {
        try
        {
            await page.WaitForURLAsync(
                url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
                new PageWaitForURLOptions { Timeout = 30000 });

            var orderId = ExtractOrderId(page.Url);

            await page.WaitForSelectorAsync(
                OrderPage.Selectors.PizzaMenuHeading,
                new PageWaitForSelectorOptions { Timeout = 30000 });

            return orderId;
        }
        catch (TimeoutException ex)
        {
            var currentContent = await page.ContentAsync();
            var currentUrl = page.Url;

            var error = $"""
                    Timed out waiting for navigation to order page.
                    This indicates the SSE message was not received.
                    Current URL: {currentUrl}.
                    Page contains error: {currentContent.Contains("error", StringComparison.OrdinalIgnoreCase)}
                    """;

            throw new InvalidOperationException(error, ex);
        }
    }

    private static Guid ExtractOrderId(string url)
    {
        var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var orderSegmentIndex = Array.IndexOf(segments, "orders");

        return Guid.Parse(segments[orderSegmentIndex + 1]);
    }
}
