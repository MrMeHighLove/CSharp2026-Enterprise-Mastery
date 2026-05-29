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
// Supplement/GrpcBidirectional.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/GrpcBidirectional.cs
// Proto definition
syntax = "proto3";
service OrderTracker {
  rpc TrackOrders (stream TrackRequest) returns (stream OrderUpdate);
}

// Server implementation
public class OrderTrackerService : OrderTracker.OrderTrackerBase
{
    private readonly IOrderEventStream _events;

    public override async Task TrackOrders(
        IAsyncStreamReader<TrackRequest> requestStream,
        IServerStreamWriter<OrderUpdate> responseStream,
        ServerCallContext context)
    {
        var trackedIds = new ConcurrentHashSet<Guid>();

        // Background: receive subscription changes from client
        var receiveTask = Task.Run(async () =>
        {
            await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
            {
                if (request.Subscribe)
                    trackedIds.Add(Guid.Parse(request.OrderId));
                else
                    trackedIds.TryRemove(Guid.Parse(request.OrderId));
            }
        }, context.CancellationToken);

        // Foreground: stream order events to client
        await foreach (var @event in _events.StreamAsync(context.CancellationToken))
        {
            if (trackedIds.Contains(@event.OrderId))
            {
                await responseStream.WriteAsync(new OrderUpdate
                {
                    OrderId = @event.OrderId.ToString(),
                    Status = @event.NewStatus,
                    Timestamp = Timestamp.FromDateTimeOffset(@event.OccurredAt)
                });
            }
        }

        await receiveTask;
    }
}

#pragma warning restore
