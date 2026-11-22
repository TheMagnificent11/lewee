using Lewee.Common;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

internal static class OrderOwnership
{
    public static bool IsOwnedByCaller(Order order, IAuthenticatedUserService authenticatedUserService)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(authenticatedUserService);

        var userId = authenticatedUserService.UserId;

        return !string.IsNullOrEmpty(userId) && string.Equals(order.UserId, userId, StringComparison.Ordinal);
    }
}
