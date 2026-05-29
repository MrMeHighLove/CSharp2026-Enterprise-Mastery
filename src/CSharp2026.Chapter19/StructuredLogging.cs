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
// Chapter19/StructuredLogging.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Program.cs: structured logging with Serilog + OpenTelemetry export
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .Enrich.WithMachineName()
       .Enrich.WithEnvironmentName()
       .WriteTo.Console(new JsonFormatter())          // structured JSON to stdout
       .WriteTo.OpenTelemetry(opts =>                 // export to OTel collector
       {
           opts.Endpoint = "http://otel-collector:4318";
           opts.ResourceAttributes = new Dictionary<string, object>
           {
               ["service.name"]    = "order-service",
               ["service.version"] = "2.1.0",
           };
       });
});

// Source-generated log messages: faster than string interpolation, and allocation-free
public static partial class OrderLog
{
    [LoggerMessage(Level = LogLevel.Information,
        Message = "Order {OrderId} created for customer {CustomerId} total {Total:C}")]
    public static partial void OrderCreated(
        ILogger logger, string orderId, string customerId, decimal total);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Order {OrderId} payment declined: {Reason}")]
    public static partial void PaymentDeclined(
        ILogger logger, string orderId, string reason);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to process order {OrderId}")]
    public static partial void OrderProcessingFailed(
        ILogger logger, Exception ex, string orderId);
}

// Usage — no string interpolation, no boxing, no allocation until logged
OrderLog.OrderCreated(_logger, order.Id.ToString(), order.CustomerId.ToString(), order.Total.Amount);

#pragma warning restore
