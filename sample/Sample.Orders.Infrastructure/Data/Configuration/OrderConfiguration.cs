using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Orders.Domain;

namespace Sample.Orders.Infrastructure.Data.Configuration;
internal class OrderConfiguration : BaseEntityConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.HasMany(x => x.Items)
            .WithMany(x => x.Orders);
    }
}
