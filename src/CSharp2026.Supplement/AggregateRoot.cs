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
// Supplement/AggregateRoot.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/AggregateRoot.cs
// Base class for all aggregate roots
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();

    protected void Raise(IDomainEvent @event) => _events.Add(@event);

    public void ClearDomainEvents() => _events.Clear();
}

// Domain events — what happened, past tense
public record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Amount,
    DateTimeOffset OccurredAt) : IDomainEvent;

public record OrderCancelled(Guid OrderId, string Reason,
    DateTimeOffset OccurredAt) : IDomainEvent;

public record OrderShipped(Guid OrderId, string TrackingNumber,
    DateTimeOffset OccurredAt) : IDomainEvent;

// Aggregate root: Order
public class Order : AggregateRoot
{
    private Order() { } // EF Core needs parameterless constructor

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money Total { get; private set; }
    private readonly List<OrderLine> _lines = [];
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    public static Order Place(Guid customerId, IEnumerable<OrderLine> lines)
    {
        var lineList = lines.ToList();
        if (lineList.Count == 0) throw new DomainException("Order must have lines");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
        };
        order._lines.AddRange(lineList);
        order.Total = Money.Sum(lineList.Select(l => l.LineTotal));

        order.Raise(new OrderPlaced(order.Id, customerId, order.Total.Amount,
            DateTimeOffset.UtcNow));

        return order;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainException("Cannot cancel a shipped order");

        Status = OrderStatus.Cancelled;
        Raise(new OrderCancelled(Id, reason, DateTimeOffset.UtcNow));
    }

    public void MarkShipped(string trackingNumber)
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Only confirmed orders can be shipped");

        Status = OrderStatus.Shipped;
        Raise(new OrderShipped(Id, trackingNumber, DateTimeOffset.UtcNow));
    }
}

// Dispatch domain events after saving (via EF Core interceptor)
public class DomainEventDispatcher : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken ct)
    {
        var aggregates = eventData.Context!.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
                await _mediator.Publish(domainEvent, ct);
            aggregate.ClearDomainEvents();
        }

        return result;
    }
}

#pragma warning restore
