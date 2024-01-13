using Lewee.Infrastructure.SqlServer;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class OrderStatusConfiguration : EnumEntityConfiguration<OrderStatus>
{
    public override string TableName => "OrderStatuses";

    public override bool IncludeZeroRecord => true;
}
