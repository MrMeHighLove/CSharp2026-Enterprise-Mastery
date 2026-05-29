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
using BenchmarkDotNet.Attributes;
using CSharp2026.Common.Domain;
using CSharp2026.Common.Events;
using CSharp2026.Common.Results;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/PerfBenchmarks.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/PerfBenchmarks.cs
[MemoryDiagnoser]
[SimpleJob]
public class StringProcessingBenchmarks
{
    private const string Input = "The quick brown fox jumps over the lazy dog";

    [Benchmark(Baseline = true)]
    public string StringConcat()
    {
        var result = "";
        foreach (var word in Input.Split(' '))
            result += word.ToUpper() + " ";
        return result.TrimEnd();
    }

    [Benchmark]
    public string StringBuilder()
    {
        var sb = new StringBuilder();
        foreach (var word in Input.Split(' '))
            sb.Append(word.ToUpper()).Append(' ');
        if (sb.Length > 0) sb.Length--;
        return sb.ToString();
    }

    [Benchmark]
    public string StringCreate()
    {
        var words = Input.Split(' ');
        var totalLen = Input.Length;
        return string.Create(totalLen, words, (span, ws) =>
        {
            int pos = 0;
            foreach (var w in ws)
            {
                w.ToUpperInvariant().AsSpan().CopyTo(span[pos..]);
                pos += w.Length;
                if (pos < span.Length) span[pos++] = ' ';
            }
        });
    }

    [Benchmark]
    public string SpanProcessing()
    {
        Span<char> buffer = stackalloc char[Input.Length];
        Input.AsSpan().CopyTo(buffer);
        for (int i = 0; i < buffer.Length; i++)
            if (buffer[i] != ' ') buffer[i] = char.ToUpper(buffer[i]);
        return buffer.ToString();
    }
}

// Relative results (run BenchmarkDotNet yourself for absolute numbers —
// they depend on CPU, runtime version, dataset, and JIT warmup):
//
// | Method         | Relative speed | Allocations          |
// |----------------|----------------|----------------------|
// | StringConcat   | baseline (1x)  | highest — grows O(n) |
// | StringBuilder  | several x      | moderate             |
// | StringCreate   | faster still   | low                  |
// | SpanProcessing | fastest        | zero heap            |
//
// The ranking is stable across hardware; the magnitudes are not.
// What matters: each step down this list removes allocation pressure.

#pragma warning restore
