using FluentAssertions;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Application.Pizzas;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class RoleAuthorizationContractTests
{
    private static readonly string[] StoreManagerRoles = [PizzaStore.Roles.StoreManagerCode];
    private static readonly string[] StoreStaffRoles = [PizzaStore.Roles.StoreStaffCode];

    [Fact]
    public void Should_AllowStoreManager_ForFulfillmentAndMenu()
    {
        IsAuthorized(StoreManagerRoles, new GetOrdersQuery().Roles).Should().BeTrue();
        IsAuthorized(StoreManagerRoles, new MarkOrderPizzasPickedUpCommand(Guid.NewGuid()).Roles)
            .Should().BeTrue();
        IsAuthorized(StoreManagerRoles, new MarkOrderPizzasDeliveredCommand(Guid.NewGuid()).Roles)
            .Should().BeTrue();
        IsAuthorized(StoreManagerRoles, new AddPizzaCommand("A", null, 1m).Roles).Should().BeTrue();
    }

    [Fact]
    public void Should_AllowStoreStaff_ForFulfillment()
    {
        IsAuthorized(StoreStaffRoles, new GetOrdersQuery().Roles).Should().BeTrue();
        IsAuthorized(StoreStaffRoles, new MarkOrderPizzasPickedUpCommand(Guid.NewGuid()).Roles)
            .Should().BeTrue();
        IsAuthorized(StoreStaffRoles, new MarkOrderPizzasDeliveredCommand(Guid.NewGuid()).Roles)
            .Should().BeTrue();
    }

    [Fact]
    public void Should_DenyStoreStaff_ForMenuManagement()
    {
        IsAuthorized(StoreStaffRoles, new AddPizzaCommand("A", null, 1m).Roles).Should().BeFalse();
        IsAuthorized(StoreStaffRoles, new EditPizzaCommand(Guid.NewGuid(), "A", null, 1m).Roles)
            .Should().BeFalse();
        IsAuthorized(StoreStaffRoles, new RemovePizzaCommand(Guid.NewGuid()).Roles).Should().BeFalse();
    }

    private static bool IsAuthorized(
        IReadOnlyCollection<string> heldRoles,
        IReadOnlyCollection<string> requiredRoles)
    {
        return heldRoles.Intersect(requiredRoles, StringComparer.Ordinal).Any();
    }
}
