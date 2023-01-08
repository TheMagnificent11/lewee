using Lewee.Infrastructure.Data;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class OrderStatusConfiguration : BaseEnumEntityConfiguration<OrderStatus>
{
    public override string TableName => "OrderStatuses";
}
