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

namespace CSharp2026.Chapter07;

#pragma warning disable
// Chapter07/StackVsHeap.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Value types (structs, record structs) avoid heap allocation in the common
// case: a local value type that is not captured or boxed stays off the heap.
public static void ProcessFrame(ReadOnlySpan<byte> frame)
{
    Vector3 position = new Vector3(1.0f, 2.0f, 3.0f);   // no heap allocation
    Quaternion rotation = Quaternion.Identity;            // no heap allocation
    // All operations on position and rotation are allocation-free

    // Transform: still allocation-free
    var transformed = Matrix4x4.CreateTranslation(position);
}

// Reference type instances are generally heap allocated
// Even small objects add GC pressure when allocated at high rates
public static void BadHotPath(IEnumerable<int> items)
{
    foreach (var item in items)
    {
        // AVOID: Allocates a new Tuple on the heap for every item
        var tuple = new Tuple<int, string>(item, item.ToString());
    }
}

public static void GoodHotPath(IEnumerable<int> items)
{
    foreach (var item in items)
    {
        // GOOD: ValueTuple is a value type — typically allocation-free here.
        // (A value type lives wherever it is declared: as a local that the
        //  JIT does not need to box or capture, it stays off the heap.
        //  Capture it in a closure or box it, and it can move to the heap.)
        var valueTuple = (item, item.ToString());
    }
}

#pragma warning restore
