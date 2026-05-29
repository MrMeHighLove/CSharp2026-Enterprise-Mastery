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
// Chapter14/StampedeProtection.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Without stampede protection (IMemoryCache naive pattern)
// 1000 concurrent requests for "product:42" all miss the cache simultaneously
// 1000 DB queries fire in parallel — database collapses under load

// GOOD: HybridCache: automatic stampede protection
// Only ONE factory invocation runs for "product:42"
// All 999 other callers wait for the single result and receive it from memory
var product = await _cache.GetOrCreateAsync(
    key: "product:42",
    factory: async token => await _db.FindByIdAsync(new ProductId(42), token),
    cancellationToken: ct);

// For legacy IMemoryCache: manual protection with SemaphoreSlim
private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

public async Task<T?> GetOrSetWithLockAsync<T>(
    string key, Func<CancellationToken, Task<T?>> factory, CancellationToken ct)
{
    if (_memCache.TryGetValue<T>(key, out var cached)) return cached;

    var keyLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
    await keyLock.WaitAsync(ct);
    try
    {
        // Double-check after acquiring lock
        if (_memCache.TryGetValue<T>(key, out cached)) return cached;
        var value = await factory(ct);
        _memCache.Set(key, value, TimeSpan.FromMinutes(5));
        return value;
    }
    finally
    {
        keyLock.Release();
    }
}

#pragma warning restore
