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

namespace CSharp2026.Chapter03;

#pragma warning disable
// Chapter03/NamingExamples.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Intent-obscuring names
public int Calc(int x, int y) => x * y / 100;

public List<int> GetData(DateTime d)
{
    var r = new List<int>();
    // ... populate r
    return r;
}

// GOOD: Intent-revealing names
public decimal CalculateDiscountedPrice(decimal unitPrice, int discountPercent)
    => unitPrice * (100 - discountPercent) / 100m;

public IReadOnlyList<OrderId> GetOrdersShippedAfter(DateOnly shippedAfterDate)
{
    var orderIds = new List<OrderId>();
    // ... populate orderIds
    return orderIds;
}

#pragma warning restore
