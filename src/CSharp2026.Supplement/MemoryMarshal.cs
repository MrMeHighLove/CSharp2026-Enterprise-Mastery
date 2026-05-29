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
// Supplement/MemoryMarshal.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/MemoryMarshal.cs
// Cast span types without copying — use with extreme care
public static ReadOnlySpan<float> AsFloats(ReadOnlySpan<byte> bytes)
{
    // MemoryMarshal.Cast: reinterprets bytes as floats (no copy)
    // Precondition: bytes.Length must be divisible by sizeof(float)
    return MemoryMarshal.Cast<byte, float>(bytes);
}

// Read a struct from a byte span (e.g., from a network packet)
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PacketHeader
{
    public ushort Version;
    public ushort Type;
    public int PayloadLength;
    public long Timestamp;
}

public static ref readonly PacketHeader ReadHeader(ReadOnlySpan<byte> data)
{
    if (data.Length < Unsafe.SizeOf<PacketHeader>())
        throw new ArgumentException("Buffer too small");
    return ref MemoryMarshal.AsRef<PacketHeader>(data);
}

// Write struct to span for network transmission
public static int WriteHeader(Span<byte> destination, in PacketHeader header)
{
    MemoryMarshal.Write(destination, in header);
    return Unsafe.SizeOf<PacketHeader>();
}

// IMPORTANT SAFETY NOTES:
// - MemoryMarshal.Cast assumes correct alignment; misaligned reads are UB on some architectures
// - These operations bypass type safety — test on all target platforms
// - Only use in hot paths where profiling proves allocation is the bottleneck
// - Document why unsafe is justified with a comment at each use site

#pragma warning restore
