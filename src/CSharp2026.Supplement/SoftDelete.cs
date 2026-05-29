// AUTO-WRAPPED for compilation. Original snippet content follows the
// namespace declaration. Snippets are illustrative and may reference
// types that need to be supplied by the chapter or by the reader.
// See ISSUES.md for the catalog of known gaps.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharp2026.Common.Domain;
using CSharp2026.Common.Events;
using CSharp2026.Common.Results;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/SoftDelete.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SoftDelete.cs
// Soft delete: mark as deleted, filter automatically everywhere
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
}

public class Order : ISoftDeletable
{
    public Guid Id { get; set; }
    // ... other properties
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

// Configure global filter in DbContext
protected override void OnModelCreating(ModelBuilder model)
{
    // Apply to all types that implement ISoftDeletable
    foreach (var entityType in model.Model.GetEntityTypes())
    {
        if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)) continue;
        model.Entity(entityType.ClrType)
             .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
    }
}

private static LambdaExpression BuildSoftDeleteFilter(Type type)
{
    // Generates: entity => !entity.IsDeleted
    var param = Expression.Parameter(type, "e");
    var prop = Expression.Property(param, nameof(ISoftDeletable.IsDeleted));
    var body = Expression.Not(prop);
    return Expression.Lambda(body, param);
}

// Override SaveChangesAsync to intercept hard deletes
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
        .Where(e => e.State == EntityState.Deleted))
    {
        entry.State = EntityState.Modified;
        entry.Entity.IsDeleted = true;
        entry.Entity.DeletedAt = DateTimeOffset.UtcNow;
    }
    return await base.SaveChangesAsync(ct);
}

// Bypass filter when needed (admin, audit, restore)
var deletedOrders = await _db.Orders.IgnoreQueryFilters()
    .Where(o => o.IsDeleted && o.DeletedAt > oneWeekAgo)
    .ToListAsync();

#pragma warning restore
