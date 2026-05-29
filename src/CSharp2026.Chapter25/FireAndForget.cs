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

namespace CSharp2026.Chapter25;

#pragma warning disable
// Chapter25/FireAndForget.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter25/FireAndForget.cs
// ANTI-PATTERN: exceptions disappear silently
_ = SendEmailAsync(user.Email); // If this throws, nothing handles it

// CORRECT: use a background task infrastructure
public class BackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue =
        Channel.CreateBounded<Func<CancellationToken, Task>>(100);

    public async ValueTask EnqueueAsync(Func<CancellationToken, Task> task)
        => await _queue.Writer.WriteAsync(task);

    public IAsyncEnumerable<Func<CancellationToken, Task>> ReadAllAsync(
        CancellationToken ct) => _queue.Reader.ReadAllAsync(ct);
}

// Worker processes tasks and logs failures properly
public class BackgroundTaskWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var task in _queue.ReadAllAsync(ct))
        {
            try { await task(ct); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background task failed");
                _metrics.RecordFailure();
            }
        }
    }
}

#pragma warning restore
