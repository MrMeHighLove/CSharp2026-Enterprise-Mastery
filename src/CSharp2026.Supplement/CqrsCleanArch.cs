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
// Supplement/CqrsCleanArch.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/CqrsCleanArch.cs
// -- Commands --------------------------------------------------------------
public record PlaceOrderCommand(
    Guid CustomerId,
    List<OrderLineDto> Lines) : IRequest<Result<Guid>>;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orders;
    private readonly IInventoryService _inventory;
    private readonly IEventBus _events;
    private readonly IUnitOfWork _uow;

    public PlaceOrderHandler(IOrderRepository orders, IInventoryService inventory,
        IEventBus events, IUnitOfWork uow)
    {
        _orders = orders;
        _inventory = inventory;
        _events = events;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        // Domain logic — no HTTP, no EF, just pure business rules
        var order = Order.Create(cmd.CustomerId, cmd.Lines.Select(LineItem.From));
        if (order.IsFailure) return Result.Failure<Guid>(order.Error);

        var reserved = await _inventory.ReserveAsync(order.Value.Lines, ct);
        if (!reserved) return Result.Failure<Guid>("Insufficient inventory");

        await _orders.AddAsync(order.Value, ct);
        await _events.PublishAsync(new OrderPlaced(order.Value.Id), ct);
        await _uow.CommitAsync(ct);

        return Result.Success(order.Value.Id);
    }
}

// -- Queries ---------------------------------------------------------------
public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderDto>>;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    private readonly IReadDbContext _read; // Separate read model — optimised

    public GetOrderHandler(IReadDbContext read) => _read = read;

    public async Task<Result<OrderDto>> Handle(GetOrderQuery q, CancellationToken ct)
    {
        var dto = await _read.Orders
            .Where(o => o.Id == q.OrderId)
            .Select(o => new OrderDto(o.Id, o.Status, o.TotalAmount, o.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return dto is not null
            ? Result.Success(dto)
            : Result.Failure<OrderDto>("Order not found");
    }
}

// -- Pipeline Behaviours ---------------------------------------------------
public class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0) throw new ValidationException(failures);
        return await next();
    }
}

#pragma warning restore
