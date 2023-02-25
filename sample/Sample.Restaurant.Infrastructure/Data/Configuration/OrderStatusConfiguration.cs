using Lewee.Infrastructure.Data;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class OrderStatusConfiguration : BaseEnumEntityConfiguration<OrderStatus>
{
    public override string TableName => "OrderStatuses";

    public override bool IncludeZeroRecord => true;
}
