﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Domain;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Domain Event Reference Configuration
/// </summary>
public class DomainEventReferenceConfiguration : IEntityTypeConfiguration<DomainEventReference>
{
    /// <summary>
    /// Configures the DomainEventReferences database table
    /// </summary>
    /// <param name="builder">
    /// Configuration builder
    /// </param>
    public void Configure(EntityTypeBuilder<DomainEventReference> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DomainEventAssemblyName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.DomainEventClassName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.DomainEventJson)
            .HasMaxLength(8000)
            .IsRequired();

        builder.Property(x => x.Dispatched)
            .IsRequired();

        builder.Property(x => x.PersistedAt)
            .IsRequired();

        builder.Property(x => x.DispatchedAt)
            .IsRequired(false);
    }
}
