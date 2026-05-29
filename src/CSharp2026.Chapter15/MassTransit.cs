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

namespace CSharp2026.Chapter15;

#pragma warning disable
// Chapter15/MassTransit.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// MassTransit with RabbitMQ: consistent API regardless of broker
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderCreatedConsumer>();
    cfg.AddConsumer<OrderShippedConsumer>();

    cfg.UsingRabbitMq((ctx, rmq) =>
    {
        rmq.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest"); h.Password("guest");
        });

        rmq.UseMessageRetry(r =>
            r.Exponential(5, TimeSpan.FromSeconds(1),
                         TimeSpan.FromSeconds(30),
                         TimeSpan.FromSeconds(2)));

        rmq.UseCircuitBreaker(cb =>
        {
            cb.TrackingPeriod  = TimeSpan.FromMinutes(1);
            cb.TripThreshold   = 15; // % error rate to trip circuit
            cb.ActiveThreshold = 10; // min active messages
            cb.ResetInterval   = TimeSpan.FromMinutes(5);
        });

        rmq.ConfigureEndpoints(ctx);
    });
});

// Publishing an event
public class OrderService
{
    private readonly IPublishEndpoint _bus;
    public OrderService(IPublishEndpoint bus) => _bus = bus;

    public async Task<OrderId> CreateAsync(OrderRequest req, CancellationToken ct)
    {
        var order = Order.Create(req.CustomerId, req.Items);
        await _repository.SaveAsync(order, ct);

        // Publish event — consumers run asynchronously
        await _bus.Publish(new OrderCreatedEvent(
            OrderId: order.Id.ToString(),
            CustomerId: order.CustomerId.ToString(),
            Total: order.Total.Amount,
            OccurredAt: DateTime.UtcNow), ct);

        return order.Id;
    }
}

// Consumer: runs independently, can be scaled separately
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IInventoryService _inventory;
    private readonly INotificationService _notify;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> ctx)
    {
        var evt = ctx.Message;
        await _inventory.ReserveItemsAsync(evt.OrderId, ctx.CancellationToken);
        await _notify.SendConfirmationAsync(evt.OrderId, evt.CustomerId, ctx.CancellationToken);
    }
}

#pragma warning restore
