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

namespace CSharp2026.Chapter23;

#pragma warning disable
// Chapter23/ParamsCollections.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter23/ParamsCollections.cs
// C# 13: params works with spans — zero allocation on the hot path
public static int Sum(params ReadOnlySpan<int> numbers)
{
    int total = 0;
    foreach (var n in numbers) total += n;
    return total;
}

// Caller syntax is unchanged — the compiler optimises the allocation
int result = Sum(1, 2, 3, 4, 5);  // No array allocated on the heap

// Works with IEnumerable<T> too for API flexibility
public void LogValues(params IEnumerable<string> values)
{
    foreach (var v in values) Console.WriteLine(v);
}

#pragma warning restore
