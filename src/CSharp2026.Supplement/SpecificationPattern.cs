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
// Supplement/SpecificationPattern.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SpecificationPattern.cs
// Base specification
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity) => ToExpression().Compile()(entity);

    public Specification<T> And(Specification<T> other) =>
        new AndSpecification<T>(this, other);

    public Specification<T> Or(Specification<T> other) =>
        new OrSpecification<T>(this, other);

    public Specification<T> Not() => new NotSpecification<T>(this);
}

// Concrete specifications — named business rules
public class PendingOrderSpec : Specification<Order>
{
    public override Expression<Func<Order, bool>> ToExpression() =>
        o => o.Status == OrderStatus.Pending;
}

public class LargeOrderSpec : Specification<Order>
{
    private readonly decimal _threshold;
    public LargeOrderSpec(decimal threshold) => _threshold = threshold;

    public override Expression<Func<Order, bool>> ToExpression() =>
        o => o.Amount >= _threshold;
}

public class OrderForCustomerSpec : Specification<Order>
{
    private readonly Guid _customerId;
    public OrderForCustomerSpec(Guid id) => _customerId = id;

    public override Expression<Func<Order, bool>> ToExpression() =>
        o => o.CustomerId == _customerId;
}

// Composite specifications
public class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left, _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var left = _left.ToExpression();
        var right = _right.ToExpression();
        var param = left.Parameters[0];
        var body = Expression.AndAlso(left.Body,
            Expression.Invoke(right, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

// Repository using specifications
public class OrderRepository
{
    public async Task<List<Order>> FindAsync(Specification<Order> spec,
        CancellationToken ct = default)
    {
        return await _db.Orders.Where(spec.ToExpression()).ToListAsync(ct);
    }
}

// Usage — composable, readable, reusable
var spec = new PendingOrderSpec()
    .And(new LargeOrderSpec(1000))
    .And(new OrderForCustomerSpec(customerId));

var orders = await _repo.FindAsync(spec);

#pragma warning restore
