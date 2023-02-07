using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Application.Data;

/// <summary>
/// Application Database Context Interface
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the domain event references database set
    /// </summary>
    DbSet<DomainEventReference>? DomainEventReferences { get; }
}
