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
// Supplement/DecoratorPattern.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/DecoratorPattern.cs
// Base interface and implementation
public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(Order order, CancellationToken ct = default);
}

// Decorator 1: Caching
public class CachingOrderRepository : IOrderRepository
{
    private readonly IOrderRepository _inner;
    private readonly IDistributedCache _cache;

    public CachingOrderRepository(IOrderRepository inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Order?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var key = $"order:{id}";
        var cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null) return JsonSerializer.Deserialize<Order>(cached);

        var order = await _inner.GetAsync(id, ct);
        if (order is not null)
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(order),
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) }, ct);
        return order;
    }

    public Task SaveAsync(Order order, CancellationToken ct = default)
        => _inner.SaveAsync(order, ct);
}

// Decorator 2: Logging
public class LoggingOrderRepository : IOrderRepository
{
    private readonly IOrderRepository _inner;
    private readonly ILogger<LoggingOrderRepository> _logger;

    public LoggingOrderRepository(IOrderRepository inner,
        ILogger<LoggingOrderRepository> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Order?> GetAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching order {OrderId}", id);
        var sw = Stopwatch.StartNew();
        var result = await _inner.GetAsync(id, ct);
        _logger.LogDebug("Fetched order {OrderId} in {Ms}ms (found={Found})",
            id, sw.ElapsedMilliseconds, result is not null);
        return result;
    }

    public async Task SaveAsync(Order order, CancellationToken ct = default)
    {
        _logger.LogInformation("Saving order {OrderId}", order.Id);
        await _inner.SaveAsync(order, ct);
    }
}

// Registration with Scrutor — order matters (outermost first)
builder.Services.AddScoped<IOrderRepository, EfCoreOrderRepository>();
builder.Services.Decorate<IOrderRepository, CachingOrderRepository>();
builder.Services.Decorate<IOrderRepository, LoggingOrderRepository>();
// Resolution chain: Logging -> Caching -> EfCore

#pragma warning restore
