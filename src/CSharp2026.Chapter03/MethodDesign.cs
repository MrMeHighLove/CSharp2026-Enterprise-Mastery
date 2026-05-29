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
// Chapter03/MethodDesign.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Mixed abstraction levels — hard to read and test
public async Task ProcessOrderAsync(OrderRequest request, CancellationToken ct)
{
    // Validate input
    if (string.IsNullOrEmpty(request.CustomerId))
        throw new ArgumentException("Customer ID required");
    if (request.Items.Count == 0)
        throw new ArgumentException("Order must have items");

    // Calculate total
    decimal total = 0;
    foreach (var item in request.Items)
        total += item.Quantity * item.UnitPrice;

    // Apply discount
    if (total > 1000)
        total *= 0.9m;

    // Save to database
    var order = new Order { CustomerId = request.CustomerId, Total = total };
    await _db.Orders.AddAsync(order, ct);
    await _db.SaveChangesAsync(ct);

    // Send confirmation email
    await _emailService.SendAsync(new OrderConfirmationEmail(order), ct);
}

// GOOD: Single level of abstraction per method
public async Task ProcessOrderAsync(OrderRequest request, CancellationToken ct)
{
    ValidateOrderRequest(request);
    var pricingSummary = CalculatePricing(request.Items);
    var order = await SaveOrderAsync(request.CustomerId, pricingSummary, ct);
    await SendConfirmationAsync(order, ct);
}

private static void ValidateOrderRequest(OrderRequest request)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(request.CustomerId, nameof(request.CustomerId));
    if (request.Items.Count == 0)
        throw new ArgumentException("Order must contain at least one item.", nameof(request.Items));
}

private static PricingSummary CalculatePricing(IReadOnlyList<OrderItem> items)
{
    var subtotal = items.Sum(i => i.Quantity * i.UnitPrice);
    var discount = subtotal > 1000m ? subtotal * 0.1m : 0m;
    return new PricingSummary(subtotal, discount, subtotal - discount);
}

#pragma warning restore
