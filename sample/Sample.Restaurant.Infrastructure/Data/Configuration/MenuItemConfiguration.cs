using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class MenuItemConfiguration : BaseAggregateRootConfiguration<MenuItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Price)
            .HasPrecision(PricePrecision.Precision, PricePrecision.Scale);

        builder.HasOne(x => x.ItemType);

        var menuItemIds = new Guid[]
        {
            new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"),
            new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"),
            new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"),
            new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"),
            new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"),
            new Guid("58b91a73-682d-4696-b545-b493b56a0335"),
            new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5")
        };

        builder.HasData(new MenuItem(menuItemIds[0], "Pizza", 15, MenuItemType.Food));
        builder.HasData(new MenuItem(menuItemIds[1], "Pasta", 15, MenuItemType.Food));
        builder.HasData(new MenuItem(menuItemIds[2], "Garlic Bread", 4.50M, MenuItemType.Food));
        builder.HasData(new MenuItem(menuItemIds[3], "Ice Cream", 5, MenuItemType.Food));
        builder.HasData(new MenuItem(menuItemIds[4], "Beer", 7.50M, MenuItemType.Drink));
        builder.HasData(new MenuItem(menuItemIds[5], "Wine", 10, MenuItemType.Drink));
        builder.HasData(new MenuItem(menuItemIds[6], "Soft Drink", 3.50M, MenuItemType.Drink));
    }
}
