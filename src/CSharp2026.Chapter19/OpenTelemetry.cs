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

namespace CSharp2026.Chapter19;

#pragma warning disable
// Chapter19/OpenTelemetry.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// OpenTelemetry distributed tracing setup
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "order-service", serviceVersion: "2.1.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation(opts =>
        {
            opts.RecordException = true;
            opts.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
        })
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(opts =>
            opts.SetDbStatementForText = true)  // captures SQL — dev only!
        .AddSource("OrderService")              // custom ActivitySource
        .AddOtlpExporter(opts =>
            opts.Endpoint = new Uri("http://otel-collector:4317")))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter("OrderService")
        .AddOtlpExporter());

// Custom spans: instrument business-critical code paths
private static readonly ActivitySource _tracer = new("OrderService");

public async Task<OrderId> ProcessAsync(OrderRequest req, CancellationToken ct)
{
    using var span = _tracer.StartActivity("ProcessOrder");
    span?.SetTag("customer.id", req.CustomerId.ToString());
    span?.SetTag("order.item_count", req.Items.Count);

    var order = Order.Create(req.CustomerId, req.Items);
    await _repository.SaveAsync(order, ct);

    span?.SetTag("order.id", order.Id.ToString());
    span?.SetTag("order.total", order.Total.Amount.ToString("F2"));
    return order.Id;
}

// Custom metrics: business-level counters and histograms
private static readonly Meter _meter = new("OrderService", "2.1.0");
private static readonly Counter<long>    _ordersCreated = _meter.CreateCounter<long>("orders.created");
private static readonly Histogram<double>_orderTotal    = _meter.CreateHistogram<double>("orders.total_amount", "USD");

public void RecordOrderCreated(Order order)
{
    _ordersCreated.Add(1, new TagList { ["status"] = "success" });
    _orderTotal.Record((double)order.Total.Amount);
}

#pragma warning restore
