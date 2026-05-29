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

namespace CSharp2026.Chapter25;

#pragma warning disable
// Chapter25/NPlusOne.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter25/NPlusOne.cs
// ANTI-PATTERN: N+1 — 1 query to get orders, then N queries for customers
var orders = await _db.Orders.ToListAsync();
foreach (var order in orders)
{
    // This executes a new query for EACH order — catastrophic at scale
    var customer = await _db.Customers.FindAsync(order.CustomerId);
    Console.WriteLine($"{customer.Name}: {order.Amount}");
}

// CORRECT: use Include() for eager loading
var orders = await _db.Orders
    .Include(o => o.Customer)
    .ToListAsync();
// Single query with JOIN — linear cost regardless of row count

// EVEN BETTER: project to DTO — load only required columns
var summaries = await _db.Orders
    .Select(o => new { o.Id, o.Amount, CustomerName = o.Customer.Name })
    .ToListAsync();

#pragma warning restore
