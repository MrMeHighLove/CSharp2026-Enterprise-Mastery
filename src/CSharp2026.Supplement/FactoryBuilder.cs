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
// Supplement/FactoryBuilder.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/FactoryBuilder.cs
// Factory method: enforce invariants at creation time
public class Order
{
    private Order() { } // Private constructor — must use factory

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderLine> Lines { get; private set; } = new();
    public decimal Total => Lines.Sum(l => l.Amount);

    // Factory: validates and constructs, returns Result
    public static Result<Order> Create(Guid customerId, IEnumerable<OrderLine> lines)
    {
        if (customerId == Guid.Empty)
            return Result.Failure<Order>("Customer ID cannot be empty");

        var lineList = lines.ToList();
        if (lineList.Count == 0)
            return Result.Failure<Order>("Order must have at least one line");

        if (lineList.Any(l => l.Quantity <= 0))
            return Result.Failure<Order>("All line quantities must be positive");

        return Result.Success(new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Draft,
            Lines = lineList
        });
    }
}

// Fluent builder for complex test data construction
public class OrderBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private OrderStatus _status = OrderStatus.Pending;
    private readonly List<OrderLine> _lines = [];

    public OrderBuilder ForCustomer(Guid id) { _customerId = id; return this; }
    public OrderBuilder WithStatus(OrderStatus s) { _status = s; return this; }

    public OrderBuilder WithLine(string sku, int qty, decimal price)
    {
        _lines.Add(new OrderLine(sku, qty, price));
        return this;
    }

    public Order Build() => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = _customerId,
        Status = _status,
        Lines = _lines
    };
}

// In tests: readable, intention-revealing
var order = new OrderBuilder()
    .ForCustomer(customer.Id)
    .WithStatus(OrderStatus.Pending)
    .WithLine("SKU-001", qty: 2, price: 49.99m)
    .WithLine("SKU-002", qty: 1, price: 99.99m)
    .Build();

#pragma warning restore
