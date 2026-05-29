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
// Chapter02/ParamsCollections.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// C# 12 and earlier — always allocates an array
void LogMessages(params string[] messages) { ... }

// C# 13 — works with IEnumerable<T>, List<T>, ReadOnlySpan<T>, etc.
void LogMessages(params ReadOnlySpan<string> messages)
{
    foreach (var msg in messages)
        Console.WriteLine(msg);
}

// Caller: no allocation when passing a span
Span<string> items = stackalloc string[] { "start", "process", "end" };
LogMessages(items);   // zero heap allocation on this call path

#pragma warning restore
