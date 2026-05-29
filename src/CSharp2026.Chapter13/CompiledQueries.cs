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
// Chapter13/CompiledQueries.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Compiled query: translate ONCE, execute thousands of times
public class OrderQueries
{
    // Static field: compiled at first use, cached for application lifetime
    private static readonly Func<AppDbContext, OrderId, CancellationToken, Task<Order?>>
        GetOrderByIdCompiled = EF.CompileAsyncQuery(
            (AppDbContext db, OrderId id, CancellationToken _) =>
                db.Orders
                  .Include(o => o.Lines)
                  .FirstOrDefault(o => o.Id == id));

    private readonly AppDbContext _db;
    public OrderQueries(AppDbContext db) => _db = db;

    // Subsequent calls skip the expression tree translation entirely
    public Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct)
        => GetOrderByIdCompiled(_db, id, ct);
}

// BenchmarkDotNet (illustrative):
// | Method              | Mean    | Notes                        |
// | RegularQuery        | 1.2 ms  | Includes LINQ translation    |
// | CompiledQuery       | 0.7 ms  | Translation cached           |
// | RawSqlQuery         | 0.5 ms  | No ORM overhead at all       |

#pragma warning restore
