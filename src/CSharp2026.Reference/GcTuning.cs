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
// Reference/GcTuning.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/GcTuning.cs
// runtimeconfig.json for server GC tuning
// {
//   "configProperties": {
//     "System.GC.Server": true,           // One GC heap per CPU core
//     "System.GC.HeapHardLimit": 2147483648, // 2GB hard memory limit
//     "System.GC.HighMemoryPercent": 90,  // Trigger GC at 90% of HeapHardLimit
//     "System.GC.ConserveMemory": 0       // 0=off, 1-9=increasingly aggressive
//   }
// }

// Monitoring GC in production
public static class GcMetricsReporter
{
    public static void LogGcStats(ILogger logger)
    {
        var info = GC.GetGCMemoryInfo();
        logger.LogInformation(
            "GC: Gen0={Gen0} Gen1={Gen1} Gen2={Gen2} " +
            "HeapSizeBytes={Heap:N0} FragmentedBytes={Frag:N0} " +
            "PauseDuration={Pause}ms",
            GC.CollectionCount(0),
            GC.CollectionCount(1),
            GC.CollectionCount(2),
            info.HeapSizeBytes,
            info.FragmentedBytes,
            info.PauseDurations.LastOrDefault().TotalMilliseconds);
    }
}

// Suppress GC during a critical section (use with extreme caution)
public static void ProcessCriticalBatch(Span<Order> orders)
{
    // Prevent GC from interrupting this batch — guarantees low latency
    // Only use for very short operations (<10ms); GC debt accrues
    GC.TryStartNoGCRegion(16 * 1024 * 1024); // 16MB budget
    try
    {
        foreach (ref var order in orders)
            ProcessOrder(ref order);
    }
    finally
    {
        GC.EndNoGCRegion();
    }
}

#pragma warning restore
