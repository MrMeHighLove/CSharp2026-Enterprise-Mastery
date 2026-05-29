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
// Supplement/SpanCookbook.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SpanCookbook.cs
// 1. Parse CSV line without allocating strings for each field
public static void ParseCsvLine(ReadOnlySpan<char> line,
    Action<ReadOnlySpan<char>, int> fieldCallback)
{
    int fieldIndex = 0;
    while (!line.IsEmpty)
    {
        int commaPos = line.IndexOf(',');
        var field = commaPos >= 0 ? line[..commaPos] : line;
        fieldCallback(field.Trim(), fieldIndex++);
        line = commaPos >= 0 ? line[(commaPos + 1)..] : ReadOnlySpan<char>.Empty;
    }
}

// 2. Parse integers from a span without ToString()
public static bool TryParseInt(ReadOnlySpan<char> span, out int result)
    => int.TryParse(span, out result);

// 3. Efficient string splitting (no array allocation)
public static void SplitOnFirst(ReadOnlySpan<char> input, char delimiter,
    out ReadOnlySpan<char> left, out ReadOnlySpan<char> right)
{
    int idx = input.IndexOf(delimiter);
    if (idx < 0) { left = input; right = ReadOnlySpan<char>.Empty; return; }
    left = input[..idx];
    right = input[(idx + 1)..];
}

// 4. Reuse buffer for multiple format operations
Span<char> buffer = stackalloc char[64];
if (orderId.TryFormat(buffer, out int written))
{
    var formatted = buffer[..written]; // Zero-allocation span view
    logger.LogDebug("Processing {OrderId}", formatted.ToString());
}

// 5. Binary protocol parsing
public static OrderHeader ParseHeader(ReadOnlySpan<byte> data)
{
    // BinaryPrimitives: reads integers from byte spans without unsafe code
    var version = BinaryPrimitives.ReadUInt16BigEndian(data[..2]);
    var messageType = BinaryPrimitives.ReadUInt16BigEndian(data[2..4]);
    var payloadLength = BinaryPrimitives.ReadInt32BigEndian(data[4..8]);
    return new OrderHeader(version, messageType, payloadLength);
}

// 6. Memory<T> for async scenarios (Span<T> cannot cross await boundaries)
public async Task<int> ProcessChunkAsync(Memory<byte> buffer, CancellationToken ct)
{
    // Memory<T> can be passed across await points
    int bytesRead = await _stream.ReadAsync(buffer, ct);
    // Slice the memory for processing
    ProcessBytes(buffer.Span[..bytesRead]); // Span view for sync processing
    return bytesRead;
}

#pragma warning restore
