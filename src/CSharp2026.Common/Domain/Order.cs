// -----------------------------------------------------------------------
//   Order.cs
//   Aggregate root. Every change goes through a method on the aggregate;
//   no setters from outside. Events are recorded as a side effect of
//   methods that change state, and dispatched after the transaction
//   commits (see Chapter 06).
// -----------------------------------------------------------------------

using CSharp2026.Common.Events;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Common.Domain;

public sealed record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId) : DomainEvent;
public sealed record OrderSubmittedEvent(OrderId OrderId, Money Total)         : DomainEvent;
public sealed record OrderCancelledEvent(OrderId OrderId, string Reason)       : DomainEvent;

public sealed class Order
{
    private readonly List<OrderLine>     _lines        = [];
    private readonly List<IDomainEvent>  _domainEvents = [];

    public OrderId     Id         { get; private set; }
    public CustomerId  CustomerId { get; private set; }
    public OrderStatus Status     { get; private set; }

    public IReadOnlyList<OrderLine>    Lines        => _lines.AsReadOnly();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public Money                       Total        =>
        _lines.Aggregate(Money.Zero, (sum, l) => sum.Add(l.Subtotal));

    private Order(OrderId id, CustomerId customerId, OrderStatus status)
    {
        Id = id;
        CustomerId = customerId;
        Status = status;
    }

    public static Order Create(CustomerId customerId, IReadOnlyList<OrderLineRequest> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new DomainException("Order must have at least one line.");
        }

        var order = new Order(OrderId.New(), customerId, OrderStatus.Draft);
        foreach (var req in lines)
        {
            order._lines.Add(OrderLine.Create(req));
        }
        order._domainEvents.Add(new OrderCreatedEvent(order.Id, order.CustomerId));
        return order;
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
        {
            throw new DomainException($"Order cannot be submitted in state {Status}.");
        }
        Status = OrderStatus.Submitted;
        _domainEvents.Add(new OrderSubmittedEvent(Id, Total));
    }

    public void Cancel(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
        {
            throw new DomainException($"Cannot cancel a {Status.ToString().ToLowerInvariant()} order.");
        }
        Status = OrderStatus.Cancelled;
        _domainEvents.Add(new OrderCancelledEvent(Id, reason));
    }

    /// <summary>
    /// Clear the recorded events. The repository calls this AFTER successfully
    /// dispatching the events (and committing the transaction) so they are
    /// not dispatched twice.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
