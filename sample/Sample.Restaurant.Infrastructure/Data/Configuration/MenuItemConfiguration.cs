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
    }
}
