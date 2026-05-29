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
// Supplement/DistributedTracing.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/DistributedTracing.cs
// Create an ActivitySource — one per library/service
public static class Telemetry
{
    public static readonly ActivitySource Source =
        new("MyApp.OrderService", "1.0.0");
}

// Add a custom span with semantic conventions
public class OrderFulfillmentService
{
    public async Task FulfillAsync(Guid orderId, CancellationToken ct)
    {
        using var activity = Telemetry.Source.StartActivity(
            "order.fulfill", ActivityKind.Internal);

        activity?.SetTag("order.id", orderId);
        activity?.SetTag("service.name", "fulfillment");

        try
        {
            // Child spans are created automatically for outbound HTTP/DB calls
            // because ASP.NET Core and EF Core instruments are registered
            var order = await _db.Orders.FindAsync(orderId, ct);
            activity?.SetTag("order.customer_id", order?.CustomerId);
            activity?.SetTag("order.amount", order?.Amount);

            await _warehouse.ReserveAsync(orderId, ct);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}

// Register ActivitySource with OpenTelemetry so spans are exported
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddSource("MyApp.*") // All ActivitySources matching this prefix
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri("http://otel-collector:4317")));

#pragma warning restore
