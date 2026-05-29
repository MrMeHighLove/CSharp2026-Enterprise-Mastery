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

namespace CSharp2026.Chapter10;

#pragma warning disable
// Chapter10/LinqPerformance.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// BenchmarkDotNet results (representative, varies by hardware):
// | Method        | Mean      | Allocated |
// | LinqSum       | 45.2 us   | 640 B     |
// | ForLoopSum    | 8.1 us    | 0 B       |
// | SpanSum       | 3.4 us    | 0 B       |

// AVOID: LINQ sum: lambda allocation + iterator overhead
int sumLinq = numbers.Where(n => n > 0).Sum();

// GOOD: For loop: no allocation, branch predictor-friendly
int sumLoop = 0;
foreach (var n in numbers)
    if (n > 0) sumLoop += n;

// GOOD: Best: Span<T> + vectorisation opportunity
int SumPositive(ReadOnlySpan<int> span)
{
    int sum = 0;
    foreach (var n in span)
        if (n > 0) sum += n;
    return sum;
}

// Rule: use LINQ for clarity in non-hot paths
//       use loops/Span in hot paths (> 10,000 calls/sec)

#pragma warning restore
