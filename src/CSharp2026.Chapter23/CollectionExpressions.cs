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
// Chapter23/CollectionExpressions.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter23/CollectionExpressions.cs
// Before C# 12:
int[] oldWay = new int[] { 1, 2, 3 };
List<string> oldList = new List<string> { "a", "b", "c" };
ImmutableArray<int> oldImmutable = ImmutableArray.Create(1, 2, 3);

// C# 12+ collection expressions — consistent syntax everywhere:
int[] array = [1, 2, 3];
List<string> list = ["a", "b", "c"];
ImmutableArray<int> immutable = [1, 2, 3];
Span<byte> span = [0x01, 0x02, 0x03];

// Spread operator merges collections:
int[] first = [1, 2, 3];
int[] second = [4, 5, 6];
int[] combined = [..first, ..second];           // [1,2,3,4,5,6]
int[] withExtra = [0, ..first, ..second, 7];    // [0,1,2,3,4,5,6,7]

// In methods — clean and expressive:
public static ReadOnlySpan<string> GetDefaultHeaders() =>
    ["Content-Type", "Authorization", "X-Request-Id"];

#pragma warning restore
