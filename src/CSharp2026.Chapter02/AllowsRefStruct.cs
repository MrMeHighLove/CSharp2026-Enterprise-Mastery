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
// Chapter02/AllowsRefStruct.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Before C# 13: could not accept Span<T> as a generic argument
// After C# 13: the allows ref struct anti-constraint permits it

static TResult Transform<T, TResult>(T input)
    where T : allows ref struct
    where TResult : allows ref struct
{
    // process input — T may be Span<byte>, ReadOnlySpan<char>, etc.
    return default!;
}

// Usage: now works with ref structs
ReadOnlySpan<char> slice = "hello world".AsSpan(0, 5);
// Generic algorithms can now process these without boxing

#pragma warning restore
