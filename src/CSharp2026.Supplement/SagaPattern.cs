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
// Supplement/SagaPattern.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SagaPattern.cs
// Choreography-based saga: each service publishes events and reacts to others
// No central coordinator — looser coupling, harder to debug

// Order service publishes OrderCreated
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand>
{
    public async Task Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var order = new Order(cmd);
        await _db.Orders.AddAsync(order, ct);
        await _db.SaveChangesAsync(ct);
        await _bus.PublishAsync(new OrderCreated(order.Id, order.Lines), ct);
    }
}

// Inventory service reacts to OrderCreated
public class ReserveInventoryConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var success = await _inventory.TryReserveAsync(context.Message.Lines);
        if (success)
            await context.Publish(new InventoryReserved(context.Message.OrderId));
        else
            await context.Publish(new InventoryReservationFailed(context.Message.OrderId));
    }
}

// Payment service reacts to InventoryReserved
public class ChargePaymentConsumer : IConsumer<InventoryReserved>
{
    public async Task Consume(ConsumeContext<InventoryReserved> context)
    {
        var result = await _payment.ChargeAsync(context.Message.OrderId);
        if (result.Success)
            await context.Publish(new PaymentCharged(context.Message.OrderId));
        else
            await context.Publish(new PaymentFailed(context.Message.OrderId));
    }
}

// Compensating transaction on failure: release reserved inventory
public class PaymentFailedConsumer : IConsumer<PaymentFailed>
{
    public async Task Consume(ConsumeContext<PaymentFailed> context)
    {
        await _inventory.ReleaseReservationAsync(context.Message.OrderId);
        await _orders.MarkFailedAsync(context.Message.OrderId);
        // Notify customer of failure
    }
}

#pragma warning restore
