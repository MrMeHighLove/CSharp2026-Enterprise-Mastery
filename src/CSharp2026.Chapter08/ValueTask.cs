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

namespace CSharp2026.Chapter08;

#pragma warning disable
// Chapter08/ValueTask.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// ValueTask avoids allocation when the result is available synchronously
// Use when: the common case completes synchronously (e.g., cache hit)
// Avoid when: the method is always async — overhead exceeds benefit

public interface ICacheService
{
    // ValueTask<T> is appropriate here — cache hits are synchronous
    ValueTask<T?> GetAsync<T>(string key, CancellationToken ct);
}

public class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _l1;
    private readonly IDistributedCache _l2;

    public async ValueTask<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        // L1 hit: synchronous — ValueTask does NOT allocate
        if (_l1.TryGetValue<T>(key, out var cached))
            return cached;

        // L2 hit: asynchronous — ValueTask wraps a Task
        var bytes = await _l2.GetAsync(key, ct);
        if (bytes is not null)
            return Deserialise<T>(bytes);

        return default;
    }

    private static T? Deserialise<T>(byte[] bytes) =>
        JsonSerializer.Deserialize<T>(bytes);
}

// CRITICAL: Never await a ValueTask more than once — store it in a variable
ValueTask<Order?> task = _cache.GetAsync<Order>("order:123", ct);
var order1 = await task;     // GOOD:
// var order2 = await task;  // AVOID: Undefined behaviour — never double-await

#pragma warning restore
