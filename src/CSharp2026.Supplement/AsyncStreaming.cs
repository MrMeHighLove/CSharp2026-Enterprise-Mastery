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
// Supplement/AsyncStreaming.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/AsyncStreaming.cs
// Repository: stream directly from EF Core's async cursor
public class OrderRepository
{
    public IAsyncEnumerable<Order> StreamLargeExportAsync(
        DateRange range, CancellationToken ct)
    {
        return _db.Orders
            .Where(o => o.CreatedAt >= range.Start && o.CreatedAt <= range.End)
            .OrderBy(o => o.CreatedAt)
            .AsAsyncEnumerable()
            .WithCancellation(ct);
    }
}

// Service: transform while streaming — no ToList()
public class ExportService
{
    public async IAsyncEnumerable<CsvRow> StreamCsvAsync(
        DateRange range,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var order in _repo.StreamLargeExportAsync(range, ct))
        {
            yield return new CsvRow(
                order.Id, order.CreatedAt, order.Amount, order.Status);
        }
    }
}

// API: stream directly to HTTP response
app.MapGet("/export/csv", async (
    DateRange range, ExportService svc,
    HttpResponse response, CancellationToken ct) =>
{
    response.ContentType = "text/csv";
    response.Headers.Append("Content-Disposition", "attachment; filename=orders.csv");

    await response.WriteAsync("Id,Date,Amount,Status
", ct);
    await foreach (var row in svc.StreamCsvAsync(range, ct))
    {
        await response.WriteAsync(
            $"{row.Id},{row.Date:yyyy-MM-dd},{row.Amount},{row.Status}
", ct);
    }
});
// Memory use is constant regardless of how many rows the query returns

#pragma warning restore
