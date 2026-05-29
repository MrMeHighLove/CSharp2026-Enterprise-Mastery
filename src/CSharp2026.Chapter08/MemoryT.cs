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
// Chapter08/MemoryT.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Memory<T> can be awaited across async boundaries
public async Task<int> ProcessBufferAsync(
    Memory<byte> buffer, CancellationToken ct)
{
    // Can await here — Memory<T> is not a ref struct
    var bytesRead = await _stream.ReadAsync(buffer, ct);

    // Get a Span<T> view for synchronous processing within this method
    var slice = buffer.Span[..bytesRead];
    return ProcessSync(slice);
}

// IMemoryOwner<T>: manage buffer lifetime with a clear ownership model
public async Task<ProcessingResult> ProcessLargePayloadAsync(
    Stream source, CancellationToken ct)
{
    // MemoryPool<T> provides pooled, owned memory
    using IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(64 * 1024);
    var bytesRead = await source.ReadAsync(owner.Memory, ct);
    return await AnalyseAsync(owner.Memory[..bytesRead], ct);
    // owner.Dispose() returns memory to the pool at end of using block
}

#pragma warning restore
