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
// Supplement/ExpressionTrees.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ExpressionTrees.cs
// Build a dynamic filter from user-provided criteria
public static Expression<Func<Order, bool>> BuildFilter(OrderSearchCriteria criteria)
{
    var param = Expression.Parameter(typeof(Order), "o");
    Expression? body = null;

    if (criteria.MinAmount.HasValue)
    {
        var prop = Expression.Property(param, nameof(Order.Amount));
        var constant = Expression.Constant(criteria.MinAmount.Value);
        var comparison = Expression.GreaterThanOrEqual(prop, constant);
        body = body is null ? comparison : Expression.AndAlso(body, comparison);
    }

    if (criteria.Status.HasValue)
    {
        var prop = Expression.Property(param, nameof(Order.Status));
        var constant = Expression.Constant(criteria.Status.Value);
        var comparison = Expression.Equal(prop, constant);
        body = body is null ? comparison : Expression.AndAlso(body, comparison);
    }

    if (criteria.CustomerId.HasValue)
    {
        var prop = Expression.Property(param, nameof(Order.CustomerId));
        var constant = Expression.Constant(criteria.CustomerId.Value);
        var comparison = Expression.Equal(prop, constant);
        body = body is null ? comparison : Expression.AndAlso(body, comparison);
    }

    // Default: match everything
    body ??= Expression.Constant(true);
    return Expression.Lambda<Func<Order, bool>>(body, param);
}

// Use in EF Core — translated to optimal SQL
public async Task<List<Order>> SearchAsync(OrderSearchCriteria criteria, CancellationToken ct)
{
    var filter = BuildFilter(criteria);
    return await _db.Orders.Where(filter).ToListAsync(ct);
}

// Dynamic sorting
public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source,
    string propertyName, bool descending = false)
{
    var param = Expression.Parameter(typeof(T), "x");
    var property = Expression.Property(param, propertyName);
    var selector = Expression.Lambda(property, param);

    var method = descending ? "OrderByDescending" : "OrderBy";
    var call = Expression.Call(typeof(Queryable), method,
        [typeof(T), property.Type],
        source.Expression, selector);

    return source.Provider.CreateQuery<T>(call);
}

// Usage: sort column from UI
var sorted = orders.OrderByDynamic(sortColumn, descending: true);

#pragma warning restore
