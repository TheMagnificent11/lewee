using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal class MenuItemConfiguration : BaseEntityConfiguration<MenuItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Price)
            .HasPrecision(6, 2);

        builder.HasMany(x => x.Orders)
            .WithMany(x => x.Items);
    }
}
