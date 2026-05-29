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
// Chapter02/NewLinqMethods.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// .NET 9: New LINQ methods
var orders = GetOrders();

// Old: GroupBy + ToDictionary — two passes, allocates groups
var oldCountByStatus = orders
    .GroupBy(o => o.Status)
    .ToDictionary(g => g.Key, g => g.Count());

// New: CountBy — single pass, no intermediate groups
var countByStatus = orders.CountBy(o => o.Status);

// New: AggregateBy — single-pass aggregate per key
var totalByCustomer = orders.AggregateBy(
    keySelector: o => o.CustomerId,
    seed: 0m,
    func: (acc, o) => acc + o.Amount);

// New: Index — get (index, element) pairs without overhead
foreach (var (i, order) in orders.Index())
{
    Console.WriteLine($"[{i}] {order.Id}");
}

#pragma warning restore
