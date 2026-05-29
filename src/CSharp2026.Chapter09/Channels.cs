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
// Chapter09/Channels.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Bounded channel: built-in backpressure — producer waits when consumer falls behind
public class OrderProcessingPipeline
{
    private readonly Channel<OrderMessage> _channel;
    private readonly ILogger _log;

    public OrderProcessingPipeline(ILogger<OrderProcessingPipeline> log)
    {
        _log = log;
        _channel = Channel.CreateBounded<OrderMessage>(
            new BoundedChannelOptions(capacity: 1000)
            {
                FullMode      = BoundedChannelFullMode.Wait,  // backpressure
                SingleReader  = false,
                SingleWriter  = false,
            });
    }

    // Producer side
    public ValueTask EnqueueAsync(OrderMessage message, CancellationToken ct)
        => _channel.Writer.WriteAsync(message, ct);

    // Consumer side — run multiple workers concurrently
    public async Task RunConsumersAsync(int workerCount, CancellationToken ct)
    {
        var workers = Enumerable.Range(0, workerCount)
            .Select(i => RunWorkerAsync(i, ct))
            .ToArray();
        await Task.WhenAll(workers);
    }

    private async Task RunWorkerAsync(int workerId, CancellationToken ct)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(ct))
        {
            try
            {
                await ProcessMessageAsync(message, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _log.LogError(ex, "Worker {WorkerId} failed processing {MessageId}",
                    workerId, message.Id);
            }
        }
    }

    public void Complete() => _channel.Writer.Complete();

    private static Task ProcessMessageAsync(OrderMessage m, CancellationToken ct)
        => Task.CompletedTask; // real implementation
}

#pragma warning restore
