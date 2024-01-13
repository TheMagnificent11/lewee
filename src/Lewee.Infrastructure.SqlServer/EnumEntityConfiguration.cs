using Lewee.Domain;
using Lewee.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.SqlServer;

/// <summary>
/// Enum Entity Configuration
/// </summary>
/// <typeparam name="TEnum">
/// Enum type
/// </typeparam>
public abstract class EnumEntityConfiguration<TEnum> : IEntityTypeConfiguration<EnumEntity<TEnum>>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Gets the table name
    /// </summary>
    public abstract string TableName { get; }

    /// <summary>
    /// Gets a value indicating whether to seed a data record with 0 for the primary key
    /// </summary>
    public abstract bool IncludeZeroRecord { get; }

    /// <summary>
    /// Configures enum entity lookup database table
    /// </summary>
    /// <param name="builder">
    /// Table builder
    /// </param>
    public virtual void Configure(EntityTypeBuilder<EnumEntity<TEnum>> builder)
    {
        builder.ToTable(this.TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        foreach (var item in Enum.GetValues<TEnum>())
        {
            if (!this.IncludeZeroRecord && item.IsEquivalentToZero())
            {
                continue;
            }

            builder.HasData(new EnumEntity<TEnum>(item));
        }
    }
}
