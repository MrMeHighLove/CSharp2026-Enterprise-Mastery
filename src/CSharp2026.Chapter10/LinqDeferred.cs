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

namespace CSharp2026.Chapter10;

#pragma warning disable
// Chapter10/LinqDeferred.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

var query = _db.Orders
    .Where(o => o.Status == OrderStatus.Pending)
    .Select(o => new { o.Id, o.CustomerId }); // Not executed yet — IQueryable

// AVOID: Executes query twice — two round trips to the database
var count = query.Count();
var first = query.FirstOrDefault();

// GOOD: Materialise once — one round trip, then work in memory
var results = await query.ToListAsync(ct);  // ONE SQL query
var count2  = results.Count;
var first2  = results.FirstOrDefault();

// AVOID: Accidental full table scan: filtering in C# instead of SQL
var active = _db.Orders.ToList()   // materialises entire table!
                       .Where(o => o.Status == OrderStatus.Pending);

// GOOD: Filter in the database — only Pending rows transferred
var activeSql = await _db.Orders
    .Where(o => o.Status == OrderStatus.Pending)
    .ToListAsync(ct);

#pragma warning restore
