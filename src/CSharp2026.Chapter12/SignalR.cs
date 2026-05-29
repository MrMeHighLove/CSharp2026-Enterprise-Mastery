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

namespace CSharp2026.Chapter12;

#pragma warning disable
// Chapter12/SignalR.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Hub — the server-side entry point for real-time connections
[Authorize]
public class OrderHub : Hub
{
    private readonly IOrderReader _reader;
    public OrderHub(IOrderReader reader) => _reader = reader;

    // Client calls this to subscribe to updates for a specific order
    public async Task SubscribeToOrder(string orderId)
    {
        var groupName = $"order:{orderId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        // Add to the customer's personal group for directed messages
        var customerId = Context.User?.FindFirst("sub")?.Value;
        if (customerId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"customer:{customerId}");
        await base.OnConnectedAsync();
    }
}

// Publishing updates from a background service
public class OrderStatusUpdater
{
    private readonly IHubContext<OrderHub> _hub;
    public OrderStatusUpdater(IHubContext<OrderHub> hub) => _hub = hub;

    public async Task NotifyStatusChangeAsync(Order order, CancellationToken ct)
    {
        var message = new OrderStatusUpdate(order.Id.ToString(), order.Status.ToString());

        // Push to all clients subscribed to this order
        await _hub.Clients
            .Group($"order:{order.Id}")
            .SendAsync("OrderStatusChanged", message, ct);
    }
}

// Registration with Redis backplane for multi-instance deployments
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, opts =>
    {
        opts.Configuration.ChannelPrefix = RedisChannel.Literal("OrderHub");
    });

#pragma warning restore
