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
// Supplement/EventSourcing.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/EventSourcing.cs
// Event store interface
public interface IEventStore
{
    Task AppendAsync(string streamId, IEnumerable<IDomainEvent> events,
        long expectedVersion, CancellationToken ct = default);
    IAsyncEnumerable<IDomainEvent> ReadAsync(string streamId,
        long fromVersion = 0, CancellationToken ct = default);
}

// Event-sourced aggregate
public abstract class EventSourcedAggregate
{
    private readonly List<IDomainEvent> _uncommittedEvents = [];
    public long Version { get; private set; } = -1;

    public IReadOnlyList<IDomainEvent> UncommittedEvents => _uncommittedEvents;

    protected void Apply(IDomainEvent @event)
    {
        When(@event); // Update state
        _uncommittedEvents.Add(@event);
        Version++;
    }

    public void Rehydrate(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            When(@event); // Replay — no uncommitted tracking
            Version++;
        }
    }

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    protected abstract void When(IDomainEvent @event);
}

// Event-sourced Order aggregate
public class OrderAggregate : EventSourcedAggregate
{
    public Guid Id { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }

    public static OrderAggregate Place(Guid customerId, decimal amount)
    {
        var order = new OrderAggregate();
        order.Apply(new OrderPlaced(Guid.NewGuid(), customerId, amount,
            DateTimeOffset.UtcNow));
        return order;
    }

    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Can only ship confirmed orders");
        Apply(new OrderShipped(Id, trackingNumber, DateTimeOffset.UtcNow));
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case OrderPlaced e:
                Id = e.OrderId;
                Status = OrderStatus.Pending;
                Total = e.Amount;
                break;
            case OrderConfirmed:
                Status = OrderStatus.Confirmed;
                break;
            case OrderShipped:
                Status = OrderStatus.Shipped;
                break;
            case OrderCancelled:
                Status = OrderStatus.Cancelled;
                break;
        }
    }
}

// Repository for event-sourced aggregates
public class EventSourcedOrderRepository
{
    private readonly IEventStore _store;

    public async Task SaveAsync(OrderAggregate order, CancellationToken ct)
    {
        var streamId = $"order-{order.Id}";
        await _store.AppendAsync(streamId, order.UncommittedEvents,
            order.Version - order.UncommittedEvents.Count, ct);
        order.ClearUncommittedEvents();
    }

    public async Task<OrderAggregate?> LoadAsync(Guid id, CancellationToken ct)
    {
        var streamId = $"order-{id}";
        var events = new List<IDomainEvent>();
        await foreach (var @event in _store.ReadAsync(streamId, ct: ct))
            events.Add(@event);

        if (events.Count == 0) return null;

        var order = new OrderAggregate();
        order.Rehydrate(events);
        return order;
    }
}

#pragma warning restore
