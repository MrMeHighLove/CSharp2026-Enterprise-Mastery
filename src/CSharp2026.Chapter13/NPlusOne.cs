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
// Chapter13/NPlusOne.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: N+1: one query for orders, N queries for each order's customer
var orders = await _db.Orders.ToListAsync(ct);
foreach (var order in orders)
{
    // EF Core lazy-loads Customer for each order — separate DB round trip!
    Console.WriteLine(order.Customer.Name);  // N additional queries
}

// GOOD: Eager loading: one JOIN query
var orders = await _db.Orders
    .Include(o => o.Customer)      // JOIN — single query
    .Include(o => o.Lines)
    .AsNoTracking()
    .ToListAsync(ct);

// GOOD: Split queries: avoids Cartesian explosion for large collections
// Instead of one massive JOIN (rows = orders * lines * products)
// EF Core runs 2–3 separate focused queries
var orders = await _db.Orders
    .Include(o => o.Lines)
        .ThenInclude(l => l.Product)
    .AsSplitQuery()   // 3 separate SELECT statements vs one massive JOIN
    .AsNoTracking()
    .ToListAsync(ct);

// GOOD: Projection: select only what you need — smallest possible payload
var summaries = await _db.Orders
    .Where(o => o.Status == OrderStatus.Active)
    .Select(o => new OrderSummaryDto(
        o.Id, o.Customer.Name, o.Lines.Sum(l => l.Total)))
    .AsNoTracking()
    .ToListAsync(ct);

#pragma warning restore
