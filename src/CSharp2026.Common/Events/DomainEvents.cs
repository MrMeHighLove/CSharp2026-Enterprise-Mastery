// -----------------------------------------------------------------------
//   DomainEvents.cs
//   Marker interface for things that happened inside an aggregate and
//   are interesting to the rest of the system.
// -----------------------------------------------------------------------

namespace CSharp2026.Common.Events;

/// <summary>
/// Something the aggregate recorded as having happened. Domain events
/// carry past tense names (OrderPlaced, PaymentCaptured) — never imperatives.
/// </summary>
public interface IDomainEvent
{
    /// <summary>UTC timestamp recorded at construction.</summary>
    DateTimeOffset OccurredAt { get; }
}

/// <summary>Convenience base record giving every event a default timestamp.</summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Thrown when an invariant of the domain model is violated. Use this for
/// rule-of-the-business errors; let infrastructure exceptions remain
/// infrastructure exceptions.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}
