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
// Supplement/EndpointModules.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/EndpointModules.cs
// IEndpointModule: convention for all endpoint groups
public interface IEndpointModule
{
    void MapEndpoints(IEndpointRouteBuilder app);
}

// OrdersModule: all order endpoints in one cohesive class
public class OrdersModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders")
            .RequireAuthorization()
            .RequireRateLimiting("api")
            .WithTags("Orders")
            .WithOpenApi();

        orders.MapGet("", GetOrders).WithName("GetOrders");
        orders.MapGet("{id:guid}", GetOrder).WithName("GetOrder");
        orders.MapPost("", PlaceOrder).WithName("PlaceOrder");
        orders.MapPut("{id:guid}", UpdateOrder).WithName("UpdateOrder");
        orders.MapDelete("{id:guid}", CancelOrder).WithName("CancelOrder");
    }

    private static async Task<IResult> GetOrders(
        [AsParameters] OrderFilterParams filter,
        ISender mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrdersQuery(filter), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> PlaceOrder(
        PlaceOrderRequest request,
        IValidator<PlaceOrderRequest> validator,
        ISender mediator,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var result = await mediator.Send(request.ToCommand(), ct);
        return result.IsSuccess
            ? Results.CreatedAtRoute("GetOrder", new { id = result.Value })
            : Results.BadRequest(result.Error);
    }

    // Other endpoint handlers...
}

// Program.cs: register all modules by assembly scanning
builder.Services.AddEndpointModules(typeof(Program).Assembly);
app.MapEndpointModules();

// Extension methods for clean registration
public static class EndpointExtensions
{
    public static IServiceCollection AddEndpointModules(
        this IServiceCollection services, Assembly assembly)
    {
        var moduleTypes = assembly.GetTypes()
            .Where(t => typeof(IEndpointModule).IsAssignableFrom(t) && !t.IsAbstract);
        foreach (var type in moduleTypes)
            services.AddSingleton(typeof(IEndpointModule), type);
        return services;
    }

    public static void MapEndpointModules(this WebApplication app)
    {
        foreach (var module in app.Services.GetServices<IEndpointModule>())
            module.MapEndpoints(app);
    }
}

#pragma warning restore
