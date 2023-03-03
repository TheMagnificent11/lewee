using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data.Configuration;

internal sealed class TableConfiguration : BaseAggregateRootConfiguration<Table>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Table> builder)
    {
        builder.HasIndex(x => x.TableNumber)
            .IsUnique();

        builder.Ignore(x => x.CurrentOrder);

        /*
        var tableIds = new List<Guid>
        {
            new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"),
            new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"),
            new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"),
            new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"),
            new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"),
            new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"),
            new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"),
            new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"),
            new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"),
            new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c")
        };

        var tableNumber = 1;
        tableIds.ForEach(x => builder.HasData(new Table(x, tableNumber++)));

        builder.Metadata
            .FindNavigation(nameof(Table.Orders))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        */
    }
}
