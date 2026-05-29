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
// Supplement/LinqDeep.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/LinqDeep.cs
// 1. Custom LINQ operator: Batch — splits a sequence into chunks
public static IEnumerable<IReadOnlyList<T>> Batch<T>(
    this IEnumerable<T> source, int batchSize)
{
    ArgumentNullException.ThrowIfNull(source);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);

    return Core(source, batchSize);

    static IEnumerable<IReadOnlyList<T>> Core(IEnumerable<T> src, int size)
    {
        var batch = new List<T>(size);
        foreach (var item in src)
        {
            batch.Add(item);
            if (batch.Count == size)
            {
                yield return batch;
                batch = new List<T>(size);
            }
        }
        if (batch.Count > 0) yield return batch;
    }
}

// Usage: process large datasets in chunks without loading everything
await foreach (var batch in allOrderIds.Batch(100).ToAsyncEnumerable())
{
    var orders = await _db.Orders.Where(o => batch.Contains(o.Id)).ToListAsync();
    await ProcessBatchAsync(orders);
}

// 2. Custom aggregation: weighted average
public static double WeightedAverage<T>(
    this IEnumerable<T> source,
    Func<T, double> value,
    Func<T, double> weight)
{
    var totalWeight = 0.0;
    var weightedSum = 0.0;
    foreach (var item in source)
    {
        var w = weight(item);
        totalWeight += w;
        weightedSum += value(item) * w;
    }
    return totalWeight == 0 ? 0 : weightedSum / totalWeight;
}

// 3. Defer expensive computation with lazy LINQ
var expensive = source
    .Where(x => x.IsEligible)         // Not yet evaluated
    .OrderByDescending(x => x.Score)  // Not yet evaluated
    .Take(10);                         // Not yet evaluated

// Everything evaluates here — once
foreach (var item in expensive) Process(item);

// 4. Short-circuiting with Any() and All()
// BAD: evaluates all 1M items
var hasHigh = orders.Where(o => o.Amount > 10_000).Any();

// GOOD: stops at first match — potentially evaluates 1 item
var hasHigh = orders.Any(o => o.Amount > 10_000);

#pragma warning restore
