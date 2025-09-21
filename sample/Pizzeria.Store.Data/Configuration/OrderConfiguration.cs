using Lewee.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

internal class OrderConfiguration : AggregateRootConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder
            .Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(Order.FieldLengths.UserId);

        builder
            .Property(x => x.DeliveryAddress)
            .HasMaxLength(Order.FieldLengths.DeliveryAddress);

        builder
            .HasMany(o => o.Pizzas)
            .WithOne(op => op.Order)
            .HasForeignKey(op => op.OrderId);
    }
}
