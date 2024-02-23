using Lewee.Infrastructure.Data;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Infrastructure.Data.Configuration;

public sealed class OrderStatusConfiguration : EnumEntityConfiguration<OrderStatus>
{
    public override string TableName => "OrderStatuses";

    public override bool IncludeZeroRecord => true;
}
