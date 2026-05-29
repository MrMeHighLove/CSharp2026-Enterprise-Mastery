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
// Supplement/JsonPerformance.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/JsonPerformance.cs
// 1. Source-generated serialiser — no reflection, AOT-compatible
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(Order[]))]
[JsonSerializable(typeof(List<Order>))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = false)]
public partial class AppJsonContext : JsonSerializerContext { }

// Register in DI
builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.AddContext<AppJsonContext>());

// Serialise without reflection
var json = JsonSerializer.Serialize(order, AppJsonContext.Default.Order);
var order = JsonSerializer.Deserialize(json, AppJsonContext.Default.Order);

// 2. Low-allocation response writing with Utf8JsonWriter
app.MapGet("/orders/bulk", async (IOrderRepository repo, HttpResponse response) =>
{
    response.ContentType = "application/json";
    await using var writer = new Utf8JsonWriter(response.Body,
        new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

    writer.WriteStartArray();
    await foreach (var order in repo.StreamAsync())
    {
        writer.WriteStartObject();
        writer.WriteNumber("id"u8, order.Id);
        writer.WriteString("status"u8, order.Status.ToString());
        writer.WriteNumber("amount"u8, order.Amount);
        writer.WriteEndObject();
    }
    writer.WriteEndArray();
    await writer.FlushAsync();
});
// Note: "status"u8 creates a UTF-8 literal — avoids encoding on every call

#pragma warning restore
