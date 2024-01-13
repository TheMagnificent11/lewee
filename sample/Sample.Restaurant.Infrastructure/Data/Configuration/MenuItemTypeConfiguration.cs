using Lewee.Infrastructure.SqlServer;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class MenuItemTypeConfiguration : EnumEntityConfiguration<MenuItemType>
{
    public override string TableName => "MenuItemTypes";

    public override bool IncludeZeroRecord => true;
}
