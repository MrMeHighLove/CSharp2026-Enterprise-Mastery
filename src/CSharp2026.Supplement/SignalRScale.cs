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
// Supplement/SignalRScale.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SignalRScale.cs
// Scale SignalR across multiple servers with Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis(
        builder.Configuration.GetConnectionString("Redis")!,
        options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal("signalr:");
        });

// Strongly-typed hub
public interface IOrderClient
{
    Task OrderStatusChanged(OrderStatusUpdate update);
    Task NewOrderAlert(NewOrderNotification notification);
}

public class OrderHub : Hub<IOrderClient>
{
    public async Task SubscribeToOrder(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"order:{orderId}");
    }

    public async Task UnsubscribeFromOrder(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order:{orderId}");
    }
}

// Push updates from a domain event handler
public class OrderStatusEventHandler : INotificationHandler<OrderStatusChanged>
{
    private readonly IHubContext<OrderHub, IOrderClient> _hub;

    public OrderStatusEventHandler(IHubContext<OrderHub, IOrderClient> hub)
        => _hub = hub;

    public async Task Handle(OrderStatusChanged notification, CancellationToken ct)
    {
        // Push to all clients tracking this order
        await _hub.Clients
            .Group($"order:{notification.OrderId}")
            .OrderStatusChanged(new OrderStatusUpdate
            {
                OrderId = notification.OrderId,
                NewStatus = notification.NewStatus,
                UpdatedAt = notification.OccurredAt
            });
    }
}

#pragma warning restore
