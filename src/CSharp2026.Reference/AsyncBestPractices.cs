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

namespace CSharp2026.Reference;

#pragma warning disable
// Reference/AsyncBestPractices.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/AsyncBestPractices.cs

// RULE 1: ConfigureAwait(false) in library code, not in app code
// Library code — avoids capturing the sync context:
public async Task<T> LibraryMethodAsync<T>(...)
{
    var result = await SomeIOAsync().ConfigureAwait(false);
    return Transform(result);
}
// Application code (ASP.NET Core, WPF) — no ConfigureAwait needed,
// ASP.NET Core has no synchronisation context by default

// RULE 2: Never block on async code
// WRONG — causes deadlock in contexts with sync context:
var result = task.Result;
var result = task.GetAwaiter().GetResult();
// RIGHT — await all the way up:
var result = await task;

// RULE 3: CancellationToken everywhere
public async Task ProcessAsync(int id, CancellationToken ct = default)
{
    var data = await _repo.GetAsync(id, ct);
    await _processor.HandleAsync(data, ct);
}

// RULE 4: ValueTask for hot paths that often complete synchronously
// Returns ValueTask when result is often available immediately (e.g. from cache)
public ValueTask<Order?> GetFromCacheAsync(Guid id)
{
    if (_cache.TryGet(id, out var order))
        return ValueTask.FromResult<Order?>(order); // No allocation
    return new ValueTask<Order?>(_repo.GetAsync(id)); // Allocates Task
}

// RULE 5: Parallel.ForEachAsync for CPU+IO hybrid workloads
await Parallel.ForEachAsync(orderIds,
    new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
    async (id, token) =>
    {
        var order = await _repo.GetAsync(id, token);
        await ProcessAsync(order, token);
    });

#pragma warning restore
