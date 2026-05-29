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
// Supplement/Projections.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/Projections.cs
// Projection: maintain a real-time order summary read model
public class OrderSummaryProjection :
    INotificationHandler<OrderPlaced>,
    INotificationHandler<OrderConfirmed>,
    INotificationHandler<OrderShipped>,
    INotificationHandler<OrderCancelled>
{
    private readonly IReadDbContext _read;

    public async Task Handle(OrderPlaced e, CancellationToken ct)
    {
        _read.OrderSummaries.Add(new OrderSummaryReadModel
        {
            Id = e.OrderId,
            CustomerId = e.CustomerId,
            Status = "Pending",
            Amount = e.Amount,
            PlacedAt = e.OccurredAt
        });
        await _read.SaveChangesAsync(ct);
    }

    public async Task Handle(OrderShipped e, CancellationToken ct)
    {
        var summary = await _read.OrderSummaries.FindAsync(e.OrderId, ct);
        if (summary is not null)
        {
            summary.Status = "Shipped";
            summary.TrackingNumber = e.TrackingNumber;
            summary.ShippedAt = e.OccurredAt;
            await _read.SaveChangesAsync(ct);
        }
    }
    // ... other handlers
}

#pragma warning restore
