﻿using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Application.Data;

/// <summary>
/// Database Context Interface
/// </summary>
public interface IDbContext : IDisposable
{
    /// <summary>
    /// Gets domain event referecnce database set
    /// </summary>
    DbSet<DomainEventReference>? DomainEventReferences { get; }

    /// <summary>
    /// Returns a queryable collection for an <see cref="IAggregateRoot"/> entity type
    /// </summary>
    /// <typeparam name="T">Aggregate root type</typeparam>
    /// <returns>A queryable collection for an <see cref="IAggregateRoot"/> entity type</returns>
    IQueryable<T> AggregateRoot<T>()
        where T : class, IAggregateRoot;

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task that contains the number of changes that were persisted to the database
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}