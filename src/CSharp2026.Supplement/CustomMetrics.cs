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
// Supplement/CustomMetrics.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/CustomMetrics.cs
// Define meters in a static class — meters are expensive to create
public static class AppMetrics
{
    private static readonly Meter _meter = new("MyApp", "1.0");

    // Counters: count things that only go up
    public static readonly Counter<long> OrdersPlaced =
        _meter.CreateCounter<long>("orders.placed", "orders",
            "Total number of orders placed");

    public static readonly Counter<long> OrdersFailed =
        _meter.CreateCounter<long>("orders.failed", "orders",
            "Total number of failed order placements");

    // Histograms: measure distributions (latency, payload size)
    public static readonly Histogram<double> OrderProcessingDuration =
        _meter.CreateHistogram<double>("orders.processing.duration", "ms",
            "Time to process an order from placement to confirmation");

    public static readonly Histogram<long> OrderAmount =
        _meter.CreateHistogram<long>("orders.amount", "cents",
            "Distribution of order amounts");

    // Gauges: snapshot values (queue depth, connection pool, active users)
    public static readonly ObservableGauge<int> PendingOrders =
        _meter.CreateObservableGauge("orders.pending", () =>
            OrderQueue.CurrentDepth,
            "orders", "Number of orders pending processing");
}

// Use metrics in business logic
public class OrderService
{
    public async Task<Guid> PlaceOrderAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var orderId = await ProcessInternalAsync(cmd, ct);

            AppMetrics.OrdersPlaced.Add(1,
                new TagList
                {
                    { "customer.tier", cmd.CustomerTier },
                    { "channel", cmd.Channel }
                });

            AppMetrics.OrderAmount.Record((long)(cmd.TotalAmount * 100));
            return orderId;
        }
        catch (Exception)
        {
            AppMetrics.OrdersFailed.Add(1,
                new TagList { { "reason", "exception" } });
            throw;
        }
        finally
        {
            AppMetrics.OrderProcessingDuration.Record(sw.Elapsed.TotalMilliseconds);
        }
    }
}

#pragma warning restore
