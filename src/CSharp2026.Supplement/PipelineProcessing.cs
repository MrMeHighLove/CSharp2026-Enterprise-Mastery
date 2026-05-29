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
// Supplement/PipelineProcessing.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/PipelineProcessing.cs
// System.IO.Pipelines: process a stream of newline-delimited JSON without buffering
public async Task ProcessJsonStreamAsync(Stream input, CancellationToken ct)
{
    var reader = PipeReader.Create(input);

    while (true)
    {
        var result = await reader.ReadAsync(ct);
        var buffer = result.Buffer;

        while (TryReadLine(ref buffer, out var line))
        {
            var order = JsonSerializer.Deserialize<Order>(line.ToArray());
            await ProcessOrderAsync(order!);
        }

        reader.AdvanceTo(buffer.Start, buffer.End);

        if (result.IsCompleted) break;
    }

    await reader.CompleteAsync();
}

private static bool TryReadLine(ref ReadOnlySequence<byte> buffer,
    out ReadOnlySequence<byte> line)
{
    var reader = new SequenceReader<byte>(buffer);
    if (reader.TryReadTo(out line, (byte)'
', advancePastDelimiter: true))
    {
        buffer = buffer.Slice(reader.Position);
        return true;
    }
    line = default;
    return false;
}

// TPL Dataflow: parallel CPU-bound processing pipeline
public async Task ProcessWithDataflowAsync(IAsyncEnumerable<Order> orders)
{
    var validateBlock = new TransformBlock<Order, Order>(
        async order => { await ValidateAsync(order); return order; },
        new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4 });

    var enrichBlock = new TransformBlock<Order, EnrichedOrder>(
        async order => await EnrichAsync(order),
        new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 8 });

    var saveBlock = new ActionBlock<EnrichedOrder>(
        async order => await SaveAsync(order),
        new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 });

    validateBlock.LinkTo(enrichBlock, new DataflowLinkOptions { PropagateCompletion = true });
    enrichBlock.LinkTo(saveBlock, new DataflowLinkOptions { PropagateCompletion = true });

    await foreach (var order in orders)
        await validateBlock.SendAsync(order);

    validateBlock.Complete();
    await saveBlock.Completion;
}

#pragma warning restore
