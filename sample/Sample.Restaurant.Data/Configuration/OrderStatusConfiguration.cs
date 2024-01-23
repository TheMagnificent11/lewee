using Lewee.Infrastructure.Data;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Data.Configuration;

internal class OrderStatusConfiguration : EnumEntityConfiguration<OrderStatus>
{
    public override string TableName => "OrderStatuses";

    public override bool IncludeZeroRecord => true;
}
