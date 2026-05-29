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

namespace CSharp2026.Chapter11;

#pragma warning disable
// Chapter11/MinimalApiComplete.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Complete, production-quality minimal API endpoint with all enterprise concerns
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHybridCache();
builder.Services.AddRateLimiter(cfg =>
{
    cfg.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.User.Identity?.Name ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, Window = TimeSpan.FromMinutes(1)
            }));
    cfg.RejectionStatusCode = 429;
});
builder.Services.AddOutputCache(cfg =>
    cfg.AddPolicy("orders", p => p.Expire(TimeSpan.FromSeconds(30))));

var app = builder.Build();
app.UseRateLimiter();
app.UseOutputCache();

// Endpoint group with shared prefix and filters
var orders = app.MapGroup("/api/v1/orders")
    .RequireAuthorization()
    .AddEndpointFilter<ValidationFilter>()
    .WithTags("Orders");

orders.MapGet("{id:guid}", async (
    Guid id, IOrderService svc, CancellationToken ct) =>
{
    var order = await svc.GetByIdAsync(new OrderId(id), ct);
    return order is null
        ? Results.NotFound()
        : Results.Ok(order);
})
.CacheOutput("orders")
.WithName("GetOrderById")
.Produces<OrderResponse>()
.Produces(404)
.WithOpenApi();

orders.MapPost("/", async (
    CreateOrderRequest req,
    IOrderService svc,
    CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.CreatedAtRoute("GetOrderById", new { id }, id);
})
.Produces<OrderId>(201)
.Produces<ValidationProblemDetails>(400)
.WithOpenApi();

#pragma warning restore
