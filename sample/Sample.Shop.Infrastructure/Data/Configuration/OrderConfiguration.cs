using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Infrastructure.Data;
using Sample.Shop.Domain.Entities;

namespace Sample.Shop.Infrastructure.Data.Configuration;

public sealed class OrderConfiguration : BaseEntityConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne(x => x.Status);

        builder.HasMany(x => x.OrderLines)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.Total);

        builder.HasIndex(x => x.CustomerId)
            .IsUnique(false);
    }
}
