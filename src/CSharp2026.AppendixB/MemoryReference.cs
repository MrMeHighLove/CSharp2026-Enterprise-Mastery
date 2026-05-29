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
// AppendixB/MemoryReference.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AppendixB/MemoryReference.cs
// Relative allocation behaviour (NOT absolute timings)

// Approach                          Heap allocation
// ----------------------------------------------------
// new int[100]                      Allocates an array on the heap
// ArrayPool<int>.Shared.Rent(100)   No allocation after pool warmup
// stackalloc int[100]               No heap allocation (stack only)
// Span<int> over existing array     No allocation (a view, not a copy)
//
// string concatenation (+) in loop  Allocates a new string each step
// StringBuilder                     Fewer allocations; one resize curve
// string.Create                     Single allocation, sized once
// Span-based string building        Zero heap allocation when sized right

// Rule of thumb: if a method is on a hot path and allocates per call,
// measure it. If it allocates per element, measure it urgently.

#pragma warning restore
