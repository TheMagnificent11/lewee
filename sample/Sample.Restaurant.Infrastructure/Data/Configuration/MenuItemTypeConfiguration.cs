using Lewee.Infrastructure.Data;
using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class MenuItemTypeConfiguration : BaseEnumEntityConfiguration<MenuItemType>
{
    public override string TableName => "MenuItemTypes";

    public override bool IncludeZeroRecord => true;
}
