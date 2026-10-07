namespace Pizzeria.Store.Application.Orders;

/// <summary>
/// Marks a request as requiring the authenticated caller to be the owner of the order.
/// </summary>
public interface IOrderOwnerRequest
{
    /// <summary>
    /// Gets the ID of the order the caller must own.
    /// </summary>
    Guid OrderId { get; }
}
