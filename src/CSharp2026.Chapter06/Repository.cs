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
// Chapter06/Repository.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Repository interface — lives in the Domain layer, knows nothing about SQL
public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(OrderId id, CancellationToken ct);
    Task SaveAsync(Order order, CancellationToken ct);
}

// EF Core implementation — lives in the Infrastructure layer
public class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    private readonly DomainEventDispatcher _dispatcher;

    public EfOrderRepository(AppDbContext db, DomainEventDispatcher dispatcher)
        => (_db, _dispatcher) = (db, dispatcher);

    public Task<Order?> FindByIdAsync(OrderId id, CancellationToken ct)
        => _db.Orders
              .Include(o => o.Lines)
              .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task SaveAsync(Order order, CancellationToken ct)
    {
        // Attach only if the change tracker is not already tracking this entity.
        // Calling Update() unconditionally marks every property modified and can
        // clobber a more precise change set EF Core already holds for a tracked
        // entity. Checking the state first keeps updates minimal and correct.
        if (_db.Entry(order).State == EntityState.Detached)
            _db.Orders.Update(order);

        await _db.SaveChangesAsync(ct);

        // Dispatch domain events AFTER the transaction commits
        await _dispatcher.DispatchAsync(order.DomainEvents, ct);
        order.ClearDomainEvents();
    }
}

#pragma warning restore
