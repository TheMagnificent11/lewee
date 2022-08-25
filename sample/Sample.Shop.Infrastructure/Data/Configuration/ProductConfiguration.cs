using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Infrastructure.Data;
using Sample.Shop.Domain.Entities;

namespace Sample.Shop.Infrastructure.Data.Configuration;

public sealed class ProductConfiguration : BaseEntityConfiguration<Product>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Product.MaxLengths.Name);

        builder.Property(x => x.Description)
            .HasMaxLength(Product.MaxLengths.Description);

        builder.Property(x => x.SalePrice)
            .HasPrecision(10, 2);

        builder.HasMany(x => x.OrdersLines)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.InStock);
    }
}
