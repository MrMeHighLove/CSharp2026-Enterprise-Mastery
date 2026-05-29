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
// Chapter13/BulkOperations.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// EF Core 7+: bulk update without loading entities into memory
// Old approach: load 10,000 orders, update in memory, save — terrible performance
var ordersToExpire = await _db.Orders
    .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < expiryCutoff)
    .ToListAsync(ct);
foreach (var o in ordersToExpire)
    o.Status = OrderStatus.Expired;
await _db.SaveChangesAsync(ct);  // 10,001 round trips (1 SELECT + N UPDATEs)

// GOOD: New: ExecuteUpdateAsync — single UPDATE ... WHERE statement
await _db.Orders
    .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < expiryCutoff)
    .ExecuteUpdateAsync(
        s => s.SetProperty(o => o.Status, OrderStatus.Expired)
              .SetProperty(o => o.UpdatedAt, DateTime.UtcNow),
        ct);
// Generates: UPDATE Orders SET Status = 'Expired', UpdatedAt = @p0
//            WHERE Status = 'Pending' AND CreatedAt < @p1

// GOOD: ExecuteDeleteAsync — single DELETE ... WHERE
await _db.Sessions
    .Where(s => s.ExpiresAt < DateTime.UtcNow)
    .ExecuteDeleteAsync(ct);

#pragma warning restore
