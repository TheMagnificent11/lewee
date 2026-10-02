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
public sealed class CheckoutFlowTests : PizzeriaTests
{
    public CheckoutFlowTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddAndRemovePizzas_And_CompletePickupCheckout()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);
        playwrightPage.Page.ShouldHaveBannerHeading();

        await playwrightPage.Page.WaitForSelectorAsync(Home.Selectors.StartOrderButton, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(Home.Selectors.StartOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.WaitForURLAsync(
            url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = 30000 });
        var orderId = ExtractOrderId(playwrightPage.Page.Url);

        await playwrightPage.Page.WaitForSelectorAsync(
            OrderPage.Selectors.PizzaMenuHeading,
            new PageWaitForSelectorOptions { Timeout = 30000 });

        // Act - add two Margherita pizzas, then remove one
        var addMargheritaSelector = $"[aria-label='add {Menu.PizzaNames.Margherita}']";
        var removeMargheritaSelector = $"[aria-label='remove {Menu.PizzaNames.Margherita}']";

        await playwrightPage.Page.WaitForSelectorAsync(addMargheritaSelector, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.ClickAsync(removeMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Proceed to checkout (in-place on the same order page)
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.CheckoutButton);

        // Submit as pickup (default selection)
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.SubmitOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert - back on order page showing order status
        await playwrightPage.Page.WaitForSelectorAsync(
            OrderPage.Selectors.OrderStatus,
            new PageWaitForSelectorOptions { Timeout = 30000 });

        var order = await this.Factory.GetOrderAsync(orderId);
        order.Should().NotBeNull();
        order.IsDeliveryOrder.Should().BeFalse();
        order.Pizzas.Should().ContainSingle(p => p.PizzaId == Menu.PizzaIds.Margherita && p.Quantity == 1);
    }

    [Fact]
    public async Task Should_CompleteDeliveryCheckout_After_ValidationFailure()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);
        playwrightPage.Page.ShouldHaveBannerHeading();

        await playwrightPage.Page.WaitForSelectorAsync(Home.Selectors.StartOrderButton, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(Home.Selectors.StartOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.WaitForURLAsync(
            url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = 30000 });
        var orderId = ExtractOrderId(playwrightPage.Page.Url);

        var addMargheritaSelector = $"[aria-label='add {Menu.PizzaNames.Margherita}']";
        await playwrightPage.Page.WaitForSelectorAsync(addMargheritaSelector, new PageWaitForSelectorOptions { Timeout = 30000 });
        await playwrightPage.Page.ClickAsync(addMargheritaSelector);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.CheckoutButton);

        // Act - select delivery without an address and submit, expecting a validation failure
        await playwrightPage.Page.ClickAsync("text=Delivery");
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.SubmitOrderButton);

        // Assert - validation error is shown and we remain on the checkout form
        await playwrightPage.Page.WaitForSelectorAsync(
            "text=Delivery address is required.",
            new PageWaitForSelectorOptions { Timeout = 10000 });
        playwrightPage.Page.Url.Should().Contain($"/orders/{orderId}");

        // Act - fill in a valid address and resubmit
        await playwrightPage.Page.FillAsync(OrderPage.Selectors.DeliveryAddressInput, "123 Pizza Street, Pizzatown");
        await playwrightPage.Page.ClickAsync(OrderPage.Selectors.SubmitOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        // Assert - order page now shows delivery status
        await playwrightPage.Page.WaitForSelectorAsync(
            OrderPage.Selectors.OrderStatus,
            new PageWaitForSelectorOptions { Timeout = 30000 });

        var orderPageContent = await playwrightPage.Page.ContentAsync();
        orderPageContent.Should().Contain("123 Pizza Street, Pizzatown");

        var order = await this.Factory.GetOrderAsync(orderId);
        order.Should().NotBeNull();
        order.IsDeliveryOrder.Should().BeTrue();
        order.DeliveryAddress.Should().Be("123 Pizza Street, Pizzatown");
    }

    private static Guid ExtractOrderId(string url)
    {
        var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var orderSegmentIndex = Array.IndexOf(segments, "orders");

        return Guid.Parse(segments[orderSegmentIndex + 1]);
    }
}
