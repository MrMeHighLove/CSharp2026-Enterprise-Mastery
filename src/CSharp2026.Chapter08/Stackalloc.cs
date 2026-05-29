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
// Chapter08/Stackalloc.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Stack-allocate small buffers — completely avoids heap allocation
public static bool TryEncodeBase64(
    ReadOnlySpan<byte> input,
    Span<byte> output,
    out int bytesWritten)
{
    // For small inputs, use a stack-allocated temporary buffer
    if (input.Length <= 256)
    {
        Span<byte> temp = stackalloc byte[Base64.GetMaxEncodedToUtf8Length(input.Length)];
        Base64.EncodeToUtf8(input, temp, out _, out bytesWritten);
        temp[..bytesWritten].CopyTo(output);
        return true;
    }
    // Fall back to ArrayPool for larger inputs
    var rented = ArrayPool<byte>.Shared.Rent(Base64.GetMaxEncodedToUtf8Length(input.Length));
    try
    {
        Base64.EncodeToUtf8(input, rented, out _, out bytesWritten);
        rented.AsSpan(0, bytesWritten).CopyTo(output);
        return true;
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(rented);
    }
}

#pragma warning restore
