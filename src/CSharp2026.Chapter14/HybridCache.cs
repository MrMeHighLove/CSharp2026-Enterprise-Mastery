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

namespace CSharp2026.Chapter14;

#pragma warning disable
// Chapter14/HybridCache.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Registration: HybridCache with L1 (in-process) and L2 (Redis) backing
builder.Services.AddHybridCache(opts =>
{
    opts.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration   = TimeSpan.FromMinutes(1),    // L1 TTL
        Expiration             = TimeSpan.FromMinutes(15),   // L2 TTL
    };
    opts.MaximumPayloadBytes = 1024 * 1024;  // 1 MB max per entry
    opts.MaximumKeyLength    = 512;
});
builder.Services.AddStackExchangeRedisCache(opts =>
    opts.Configuration = redisConnectionString);

// Usage: GetOrCreateAsync handles L1 hit, L2 hit, and DB miss automatically
public class ProductCatalogue
{
    private readonly IHybridCache _cache;
    private readonly IProductReader _db;

    public ProductCatalogue(IHybridCache cache, IProductReader db)
        => (_cache, _db) = (cache, db);

    public async ValueTask<Product?> GetByIdAsync(ProductId id, CancellationToken ct)
        => await _cache.GetOrCreateAsync(
            key: $"product:{id}",
            factory: async token => await _db.FindByIdAsync(id, token),
            cancellationToken: ct);

    // Tag-based invalidation: invalidate all products in a category
    public async Task InvalidateCategoryAsync(string category, CancellationToken ct)
        => await _cache.RemoveByTagAsync(category, ct);

    // Cache with tags for fine-grained invalidation
    public async ValueTask<IReadOnlyList<Product>> GetByCategoryAsync(
        string category, CancellationToken ct)
        => await _cache.GetOrCreateAsync(
            key: $"products:cat:{category}",
            factory: async token => await _db.GetByCategoryAsync(category, token),
            options: new HybridCacheEntryOptions { Tags = [category] },
            cancellationToken: ct) ?? [];
}

#pragma warning restore
