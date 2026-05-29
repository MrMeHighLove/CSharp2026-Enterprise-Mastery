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
// Supplement/EfCoreAdvanced.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/EfCoreAdvanced.cs
// 1. Query splitting: prevents cartesian explosion on multi-collection includes
// Without splitting: Orders JOIN Lines JOIN Products JOIN Tags = huge cartesian product
var orders = await _db.Orders
    .AsSplitQuery()  // Executes separate queries, joins in memory
    .Include(o => o.Lines)
        .ThenInclude(l => l.Product)
    .Include(o => o.Tags)
    .Where(o => o.CustomerId == customerId)
    .ToListAsync();

// 2. No-tracking queries for read-only scenarios (noticeably faster - EF skips snapshotting)
var summaries = await _db.Orders
    .AsNoTracking()  // EF won't snapshot these entities
    .Where(o => o.Status == OrderStatus.Pending)
    .Select(o => new OrderSummary(o.Id, o.CreatedAt, o.Total))
    .ToListAsync();

// 3. Compiled queries: reuse the LINQ->SQL translation (significant savings at scale)
private static readonly Func<AppDbContext, Guid, Task<Order?>> _getOrderById =
    EF.CompileAsyncQuery((AppDbContext db, Guid id) =>
        db.Orders.FirstOrDefault(o => o.Id == id));

// Usage — zero LINQ compilation overhead
var order = await _getOrderById(_db, orderId);

// 4. Raw SQL for complex queries EF can't optimise
var leaderboard = await _db.Database
    .SqlQueryRaw<CustomerLeaderboard>(@"
        SELECT c.Id, c.Name,
               COUNT(o.Id) AS OrderCount,
               SUM(o.Amount) AS TotalSpend,
               RANK() OVER (ORDER BY SUM(o.Amount) DESC) AS Rank
        FROM Customers c
        JOIN Orders o ON o.CustomerId = c.Id
        WHERE o.CreatedAt >= @start
        GROUP BY c.Id, c.Name
        ORDER BY Rank
        LIMIT @take
        "",
        new NpgsqlParameter("start", DateTimeOffset.UtcNow.AddMonths(-1)),
        new NpgsqlParameter("take", 100))
    .ToListAsync();

// 5. ExecuteUpdateAsync / ExecuteDeleteAsync: bulk ops without loading entities
// Before (.NET 6 and earlier): load 10k entities, modify, SaveChanges
// After (.NET 7+): single UPDATE statement
var updated = await _db.Orders
    .Where(o => o.Status == OrderStatus.PendingPayment
                && o.CreatedAt < DateTime.UtcNow.AddDays(-7))
    .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, OrderStatus.Expired));

_logger.LogInformation("Expired {Count} orders", updated);

#pragma warning restore
