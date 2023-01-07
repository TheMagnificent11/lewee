using Lewee.Application.Auth;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Audit Details Save Changes Interceptor
/// </summary>
/// <remarks>
/// Sets audit properties on <see cref="BaseEntity"/> objects before database context changes are saved
/// </remarks>
public class AuditDetailsSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IAuthenticatedUserService authenticatedUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditDetailsSaveChangesInterceptor"/> class
    /// </summary>
    /// <param name="authenticatedUserService">Authenticated user service</param>
    public AuditDetailsSaveChangesInterceptor(IAuthenticatedUserService authenticatedUserService)
    {
        this.authenticatedUserService = authenticatedUserService;
    }

    /// <summary>
    /// Saves changes interceptor
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="result">Result</param>
    /// <returns>Interception result</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        this.UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Saves changes interceptor
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="result">Result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asynchronous task contains an interception result</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        this.UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Update entities
    /// </summary>
    /// <param name="context">Database context</param>
    public void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.ApplyCreationTrackingData(this.authenticatedUserService.UserId);
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.ApplyModificationTrackingData(this.authenticatedUserService.UserId);
            }
        }
    }
}
