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
// Supplement/CacheInvalidation.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/CacheInvalidation.cs
// 1. Event-driven invalidation via MediatR notification
public record OrderUpdated(Guid OrderId) : INotification;

public class OrderCacheInvalidator : INotificationHandler<OrderUpdated>
{
    private readonly IDistributedCache _cache;

    public OrderCacheInvalidator(IDistributedCache cache) => _cache = cache;

    public async Task Handle(OrderUpdated notification, CancellationToken ct)
    {
        // Remove all cache keys related to this order
        var keys = new[]
        {
            $"order:{notification.OrderId}",
            $"order-summary:{notification.OrderId}",
            $"customer-orders:{GetCustomerIdForOrder(notification.OrderId)}",
        };
        foreach (var key in keys)
            await _cache.RemoveAsync(key, ct);
    }
}

// 2. Versioned cache keys — never stale, never need explicit invalidation
public class VersionedCache
{
    private readonly IDistributedCache _cache;
    private readonly IEntityVersionStore _versions;

    public async Task<T?> GetAsync<T>(string entityType, Guid id, CancellationToken ct)
    {
        var version = await _versions.GetVersionAsync(entityType, id, ct);
        var key = $"{entityType}:{id}:v{version}";
        var json = await _cache.GetStringAsync(key, ct);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string entityType, Guid id, T value, CancellationToken ct)
    {
        var version = await _versions.IncrementAsync(entityType, id, ct);
        var key = $"{entityType}:{id}:v{version}";
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(value),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            }, ct);
    }
}

// 3. Write-through cache: update cache atomically with the database
public class OrderService
{
    public async Task UpdateOrderAsync(Guid id, UpdateOrderDto dto, CancellationToken ct)
    {
        var order = await _db.Orders.FindAsync(id, ct);
        order!.UpdateFrom(dto);
        await _db.SaveChangesAsync(ct);

        // Update cache immediately — read-after-write consistency
        var cacheKey = $"order:{id}";
        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(OrderDto.From(order)),
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) },
            ct);
    }
}

#pragma warning restore
