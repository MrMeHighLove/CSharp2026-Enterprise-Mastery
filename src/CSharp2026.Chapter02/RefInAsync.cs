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

namespace CSharp2026.Chapter02;

#pragma warning disable
// Chapter02/RefInAsync.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// C# 13: ref locals and Span<T> may be used inside an async method.
public async Task<int> ProcessBufferAsync(Memory<byte> memory, CancellationToken ct)
{
    await Task.Yield();                 // an await boundary

    // OK: the ref local and the Span<T> are created, used, and discarded
    // entirely WITHIN this synchronous region — they never cross an await.
    int processed = MutateInPlace(memory.Span);

    await SomeAsyncOperation(ct);        // another await boundary
    return processed;
}

// The Span<T> work lives in a synchronous helper. This is the pattern that
// always compiles, regardless of C# version, and it documents intent clearly.
private static int MutateInPlace(Span<byte> buffer)
{
    if (buffer.IsEmpty) return 0;
    ref byte firstByte = ref buffer[0];
    firstByte = 0xFF;                   // direct memory mutation
    return buffer.Length;
}

// WHY NOT keep the ref local in the async method directly?
// Because a ref local (or Span<T>) cannot survive an await: after the method
// suspends and resumes, the referenced storage may have moved. The compiler
// rejects a ref local that is still in scope across an await. Confining the
// ref work to a synchronous helper makes that boundary explicit.

#pragma warning restore
