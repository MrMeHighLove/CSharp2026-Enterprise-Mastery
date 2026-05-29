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
// Chapter12/OrderGrpcService.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// C# gRPC service implementation
public class OrderGrpcService : OrderService.OrderServiceBase
{
    private readonly IOrderReader _reader;
    public OrderGrpcService(IOrderReader reader) => _reader = reader;

    public override async Task<OrderResponse> GetOrder(
        GetOrderRequest request,
        ServerCallContext context)
    {
        var order = await _reader.GetByIdAsync(
            new OrderId(Guid.Parse(request.OrderId)),
            context.CancellationToken);

        if (order is null)
        {
            var metadata = new Metadata {{ "order-id", request.OrderId }};
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Order {request.OrderId} not found"), metadata);
        }
        return MapToResponse(order);
    }

    // Server streaming: client receives a stream of orders
    public override async Task ListOrders(
        ListRequest request,
        IServerStreamWriter<OrderResponse> responseStream,
        ServerCallContext context)
    {
        var orders = await _reader.GetByCustomerAsync(
            new CustomerId(Guid.Parse(request.CustomerId)),
            context.CancellationToken);

        foreach (var order in orders)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            await responseStream.WriteAsync(MapToResponse(order));
        }
    }

    private static OrderResponse MapToResponse(Order o) => new()
    {
        OrderId    = o.Id.ToString(),
        CustomerId = o.CustomerId.ToString(),
        TotalAmount= (double)o.Total.Amount,
        Status     = o.Status.ToString(),
        CreatedAt  = new DateTimeOffset(o.CreatedAt).ToUnixTimeSeconds(),
    };
}

#pragma warning restore
