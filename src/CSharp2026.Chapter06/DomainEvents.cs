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
// Chapter06/DomainEvents.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Domain event — value object describing something that happened
public record OrderSubmittedEvent(OrderId OrderId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

// Dispatcher: publish after the transaction commits
public class DomainEventDispatcher
{
    private readonly IServiceProvider _sp;
    public DomainEventDispatcher(IServiceProvider sp) => _sp = sp;

    public async Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var evt in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(evt.GetType());
            var handlers = _sp.GetServices(handlerType);
            foreach (dynamic handler in handlers)
                await handler.HandleAsync((dynamic)evt, ct);
        }
    }
}

// Handler: runs outside the transaction — side effects isolated
public class SendOrderConfirmationOnSubmit : IDomainEventHandler<OrderSubmittedEvent>
{
    private readonly IOrderNotifier _notifier;
    public SendOrderConfirmationOnSubmit(IOrderNotifier notifier) => _notifier = notifier;

    public async Task HandleAsync(OrderSubmittedEvent evt, CancellationToken ct)
        => await _notifier.SendSubmissionConfirmationAsync(evt.OrderId, ct);
}

#pragma warning restore
