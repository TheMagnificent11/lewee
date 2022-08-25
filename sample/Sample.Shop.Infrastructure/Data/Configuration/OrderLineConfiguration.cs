using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Infrastructure.Data;
using Sample.Shop.Domain.Entities;

namespace Sample.Shop.Infrastructure.Data.Configuration;

public sealed class OrderLineConfiguration : BaseEntityConfiguration<OrderLine>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OrderLine> builder)
    {
        builder.Property(x => x.Price)
            .HasPrecision(10, 2);

        builder.Property(x => x.Cost)
            .HasPrecision(10, 2);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderLines)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrdersLines)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
