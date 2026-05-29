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

namespace CSharp2026.Chapter06;

#pragma warning disable
// Chapter06/Entities.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Value Object: immutable, structural equality, no database identity
public record Money(decimal Amount, string Currency)
{
    public static readonly Money Zero = new(0m, "USD");

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return this with { Amount = Amount + other.Amount };
    }
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        if (Amount < other.Amount)
            throw new DomainException("Cannot result in negative money.");
        return this with { Amount = Amount - other.Amount };
    }
    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Currency mismatch: {Currency} vs {other.Currency}");
    }
}

// Entity: has identity, mutable state, domain behaviour
public class Order
{
    private readonly List<OrderLine> _lines = [];

    private Order() { }  // EF Core navigation

    public OrderId     Id          { get; private set; }
    public CustomerId  CustomerId  { get; private set; }
    public OrderStatus Status      { get; private set; }
    public Money       Total       => _lines.Aggregate(Money.Zero, (sum, l) => sum.Add(l.Subtotal));
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent>  DomainEvents => _domainEvents.AsReadOnly();

    public static Order Create(CustomerId customerId, IReadOnlyList<OrderLineRequest> lines)
    {
        ArgumentNullException.ThrowIfNull(customerId, nameof(customerId));
        if (lines.Count == 0) throw new DomainException("Order must have at least one line.");

        var order = new Order
        {
            Id         = OrderId.New(),
            CustomerId = customerId,
            Status     = OrderStatus.Draft,
        };
        foreach (var req in lines)
            order._lines.Add(OrderLine.Create(req));

        order._domainEvents.Add(new OrderCreatedEvent(order.Id, order.CustomerId));
        return order;
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException($"Cannot submit an order in status {Status}.");
        Status = OrderStatus.Submitted;
        _domainEvents.Add(new OrderSubmittedEvent(Id));
    }
}

#pragma warning restore
