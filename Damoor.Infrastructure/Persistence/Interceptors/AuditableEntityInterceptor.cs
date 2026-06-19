using Damoor.Domain.Common;
using Damoor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Damoor.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Deleted &&
                entry.Entity is INonDeletable)
            {
                throw new InvalidOperationException(
                    $"{entry.Metadata.ClrType.Name} records cannot be deleted.");
            }

            if (entry.State == EntityState.Deleted &&
                entry.Entity is ISoftDeletable softDeletable)
            {
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;

                if (entry.Entity is ISoftDeletable addedSoftDeletable)
                {
                    addedSoftDeletable.IsDeleted = false;
                    addedSoftDeletable.DeletedAt = null;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;

                if (entry.Entity is ISoftDeletable modifiedSoftDeletable)
                {
                    NormalizeSoftDeleteState(
                        modifiedSoftDeletable,
                        utcNow);
                }
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<AppUser>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.IsDeleted = false;
                entry.Entity.DeletedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
                NormalizeSoftDeleteState(entry.Entity, utcNow);
            }
        }
    }

    private static void NormalizeSoftDeleteState(
        ISoftDeletable entity,
        DateTime utcNow)
    {
        if (entity.IsDeleted)
        {
            entity.DeletedAt ??= utcNow;
        }
        else
        {
            entity.DeletedAt = null;
        }
    }
}
