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

namespace CSharp2026.Chapter07;

#pragma warning disable
// Chapter07/ArrayPool.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Allocates 4KB on the heap for every call - at high request rates this is severe GC pressure
public static async Task<string> ReadResponseAsync(Stream stream, CancellationToken ct)
{
    var buffer = new byte[4096];   // heap allocation every call
    var read = await stream.ReadAsync(buffer, ct);
    return Encoding.UTF8.GetString(buffer, 0, read);
}

// GOOD: Rent and return — zero steady-state allocation
public static async Task<string> ReadResponseAsync(Stream stream, CancellationToken ct)
{
    var buffer = ArrayPool<byte>.Shared.Rent(4096);
    try
    {
        var read = await stream.ReadAsync(buffer, ct);
        return Encoding.UTF8.GetString(buffer, 0, read);
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer, clearArray: false);
    }
}

// GOOD: Even better: use RecyclableMemoryStream for variable-length payloads
// Microsoft.IO.RecyclableMemoryStream pools both arrays and MemoryStream instances
using var ms = _memoryStreamManager.GetStream("request-body");
await request.Body.CopyToAsync(ms, ct);
var payload = ms.ToArray();  // or process the MemoryStream directly

#pragma warning restore
