using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lewee.Infrastructure.SqlServer;

internal class AuditDetailsSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IAuthenticatedUserService authenticatedUserService;

    public AuditDetailsSaveChangesInterceptor(IAuthenticatedUserService authenticatedUserService)
    {
        this.authenticatedUserService = authenticatedUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        this.UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        this.UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
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
