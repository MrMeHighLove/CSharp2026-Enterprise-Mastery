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
// Chapter10/QueryableVsEnumerable.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// IQueryable<T>: expression trees — translated to SQL or other query languages
// IEnumerable<T>: delegates — executed in memory (C#)

// The mix-up that costs teams dearly:
IQueryable<Order> dbQuery = _db.Orders.Where(o => o.Status == OrderStatus.Pending);

// GOOD: Stay in IQueryable — condition translated to SQL WHERE clause
var sqlFiltered = await dbQuery
    .Where(o => o.CreatedAt > DateTime.UtcNow.AddDays(-7))  // SQL: WHERE created_at > ...
    .ToListAsync(ct);

// AVOID: Materialise accidentally — AsEnumerable() pulls every row to memory first
var cSharpFiltered = dbQuery
    .AsEnumerable()  // <<< query executes HERE; all rows loaded into memory
    .Where(o => o.CreatedAt > DateTime.UtcNow.AddDays(-7))  // in-memory C# filter
    .ToList();       // synchronous — the data is already in memory

// Why this is a trap:
//  - After AsEnumerable(), you are in LINQ-to-Objects, not LINQ-to-Entities.
//  - The WHERE never reaches SQL; the database returns the whole table.
//  - There is no ToListAsync() here because IEnumerable<T> has no async
//    enumeration; the await belongs on the database call, not the C# filter.
//
// AsEnumerable() has legitimate uses (a condition EF Core cannot translate),
// but place it AFTER every filter the database can run, never before.

#pragma warning restore
