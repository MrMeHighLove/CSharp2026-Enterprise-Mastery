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

namespace CSharp2026.Chapter13;

#pragma warning disable
// Chapter13/QueryLogging.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Enable EF Core query logging during development
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlServer(connectionString);
    if (builder.Environment.IsDevelopment())
    {
        opts.LogTo(Console.WriteLine, LogLevel.Information);
        opts.EnableSensitiveDataLogging();  // shows parameter values — only in dev!
        opts.EnableDetailedErrors();
    }
});

// Using .TagWith() to identify queries in the database profiler
var orders = await _db.Orders
    .TagWith("GetActiveOrders_v2 called from OrderDashboardController")
    .Where(o => o.Status == OrderStatus.Active && o.CreatedAt > cutoff)
    .AsNoTracking()  // CRITICAL for read-only queries — no change tracking overhead
    .ToListAsync(ct);

#pragma warning restore
