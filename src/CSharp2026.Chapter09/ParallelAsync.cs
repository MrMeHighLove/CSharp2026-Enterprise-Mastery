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

namespace CSharp2026.Chapter09;

#pragma warning disable
// Chapter09/ParallelAsync.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Process up to 8 items concurrently, respecting cancellation
public static async Task ProcessOrdersBatchAsync(
    IAsyncEnumerable<Order> orders,
    CancellationToken ct)
{
    var options = new ParallelOptions
    {
        MaxDegreeOfParallelism = 8,
        CancellationToken      = ct
    };

    await Parallel.ForEachAsync(orders, options, async (order, token) =>
    {
        await ProcessSingleOrderAsync(order, token);
    });
}

// SemaphoreSlim: control concurrency without Parallel.ForEachAsync
public static async Task ProcessWithThrottleAsync(
    IReadOnlyList<Guid> ids, int maxConcurrent, CancellationToken ct)
{
    using var throttle = new SemaphoreSlim(maxConcurrent, maxConcurrent);

    var tasks = ids.Select(async id =>
    {
        await throttle.WaitAsync(ct);
        try   { await ProcessAsync(id, ct); }
        finally { throttle.Release(); }
    });

    await Task.WhenAll(tasks);
}

private static Task ProcessSingleOrderAsync(Order o, CancellationToken ct) => Task.CompletedTask;
private static Task ProcessAsync(Guid id, CancellationToken ct) => Task.CompletedTask;

#pragma warning restore
