using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Infrastructure.Data.Configuration;

public sealed class OrderConfiguration : EntityConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.DeliveryAddress)
            .HasMaxLength(Order.DeliveryAddressMaxLength);

        builder.HasOne(x => x.Status);
    }
}
