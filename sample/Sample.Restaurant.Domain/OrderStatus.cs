using System.ComponentModel;

namespace Sample.Restaurant.Domain;

public enum OrderStatus
{
    Ordering = 0,

    [Description("Order Placed")]
    OrderPlaced = 1,

    Updated = 2,

    Paid = 3
}
