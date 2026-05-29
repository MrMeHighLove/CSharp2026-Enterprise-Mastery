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
// Supplement/StructEnumerator.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/StructEnumerator.cs
// Custom struct enumerator: zero allocation, foreach-compatible
public readonly struct RangeEnumerable
{
    private readonly int _start;
    private readonly int _end;
    private readonly int _step;

    public RangeEnumerable(int start, int end, int step = 1)
    {
        _start = start;
        _end = end;
        _step = step;
    }

    public Enumerator GetEnumerator() => new Enumerator(_start, _end, _step);

    public struct Enumerator
    {
        private int _current;
        private readonly int _end;
        private readonly int _step;

        internal Enumerator(int start, int end, int step)
        {
            _current = start - step;
            _end = end;
            _step = step;
        }

        public int Current => _current;

        public bool MoveNext()
        {
            _current += _step;
            return _current < _end;
        }
    }
}

// Usage — identical to foreach over IEnumerable<int> but zero allocs
foreach (var i in new RangeEnumerable(0, 1_000_000, 2))
{
    Process(i);
}

// The compiler uses duck typing: GetEnumerator() returning a struct
// with Current/MoveNext is sufficient — no IEnumerable needed

#pragma warning restore
