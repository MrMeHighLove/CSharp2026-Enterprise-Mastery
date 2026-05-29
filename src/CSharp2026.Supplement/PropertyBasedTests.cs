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
// Supplement/PropertyBasedTests.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/PropertyBasedTests.cs
using FsCheck;
using FsCheck.Xunit;

public class OrderCalculatorProperties
{
    // Property: total is always the sum of line totals
    [Property]
    public Property TotalEqualsSumOfLines(List<(decimal Price, int Qty)> lines)
    {
        var orderLines = lines
            .Where(l => l.Price > 0 && l.Qty > 0)
            .Select(l => new OrderLine(l.Price, l.Qty))
            .ToList();

        var order = new Order(orderLines);

        var expected = orderLines.Sum(l => l.Price * l.Qty);
        return (order.Total == expected)
            .Label($"Expected {expected}, got {order.Total}");
    }

    // Property: applying a discount never increases the total
    [Property]
    public Property DiscountNeverIncreasesTotal(
        PositiveInt amount,
        NonNegativeInt discountPercent)
    {
        var pct = discountPercent.Get % 101; // 0..100
        var order = new Order { Amount = amount.Get };
        var discounted = order.ApplyDiscount(pct);

        return (discounted.Amount <= order.Amount)
            .Label($"Discount {pct}%: {order.Amount} -> {discounted.Amount}");
    }

    // Property: serialise/deserialise round-trip preserves equality
    [Property]
    public Property SerialiseRoundTrip(Order order)
    {
        var json = JsonSerializer.Serialize(order);
        var restored = JsonSerializer.Deserialize<Order>(json);
        return (order == restored)
            .Label($"Round trip failed for {json}");
    }
}

#pragma warning restore
