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

namespace CSharp2026.AppendixB;

#pragma warning disable
// AppendixB/CollectionReference.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AppendixB/CollectionReference.cs
// Algorithmic complexity (stable across all hardware)

// Operation                     Complexity   Notes
// --------------------------------------------------------------
// List<T> Add                   O(1)*        amortised; O(n) on resize
// List<T> indexer  [i]          O(1)
// List<T> Contains              O(n)         linear scan
// Dictionary<K,V> lookup        O(1)*        amortised; hash-dependent
// HashSet<T> Contains           O(1)*        amortised
// Array binary search           O(log n)     requires sorted array
// SortedList/SortedSet lookup   O(log n)

// Pre-size collections when the count is known: it removes the
// resize curve entirely. new List<int>(expectedCount) is free insurance.

// LINQ vs. hand-written loops: LINQ adds delegate-invocation and
// iterator overhead per element. For most code this is irrelevant and
// LINQ's clarity wins. On measured hot paths, a plain loop or a
// Span<T> iteration removes that overhead. Measure before rewriting.

#pragma warning restore
