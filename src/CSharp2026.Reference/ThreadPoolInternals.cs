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
// Reference/ThreadPoolInternals.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/ThreadPoolInternals.cs
// The .NET thread pool uses a work-stealing algorithm:
// Each CPU core has its own queue; idle threads steal from busy ones.
// This gives excellent locality and minimal contention.

// Monitor thread pool saturation — a leading indicator of performance problems
public class ThreadPoolMonitor : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            ThreadPool.GetAvailableThreads(out int availWorker, out int availIO);
            ThreadPool.GetMaxThreads(out int maxWorker, out int maxIO);
            ThreadPool.GetMinThreads(out int minWorker, out int minIO);

            var workerUtil = (maxWorker - availWorker) * 100.0 / maxWorker;
            var ioUtil = (maxIO - availIO) * 100.0 / maxIO;

            if (workerUtil > 80)
                _logger.LogWarning(
                    "Thread pool worker utilisation: {Pct:F0}% ({Used}/{Max})",
                    workerUtil, maxWorker - availWorker, maxWorker);

            _metrics.RecordThreadPoolUtilisation(workerUtil, ioUtil);
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
        }
    }
}

// Avoid thread pool starvation:
// - Never block a thread pool thread synchronously (no .Result, no Thread.Sleep)
// - Limit unbounded parallelism (use SemaphoreSlim or Parallel.ForEachAsync)
// - Prefer async I/O over synchronous I/O in all hot paths
// - Consider increasing min threads for burst workloads:
ThreadPool.SetMinThreads(workerThreads: 100, completionPortThreads: 100);
// But: more threads = more context switching overhead; profile first

#pragma warning restore
