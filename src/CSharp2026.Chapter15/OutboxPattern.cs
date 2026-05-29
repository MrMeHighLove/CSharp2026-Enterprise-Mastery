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
// Chapter15/OutboxPattern.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Outbox pattern with EF Core: atomic business data + event storage
public class OutboxOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OutboxOrderRepository(AppDbContext db) => _db = db;

    public async Task SaveAsync(Order order, CancellationToken ct)
    {
        // Both operations in ONE transaction — atomically consistent
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // Attach only if not already tracked (see Chapter 6 — avoid
            // marking every property modified on an already-tracked entity).
            if (_db.Entry(order).State == EntityState.Detached)
                _db.Orders.Update(order);

            // Write events to the outbox table — same transaction
            foreach (var evt in order.DomainEvents)
            {
                _db.OutboxMessages.Add(new OutboxMessage
                {
                    Id          = Guid.NewGuid(),
                    Type        = evt.GetType().AssemblyQualifiedName!,
                    Payload     = JsonSerializer.Serialize(evt, evt.GetType()),
                    CreatedAt   = DateTime.UtcNow,
                    ProcessedAt = null,
                });
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            order.ClearDomainEvents();
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}

// Background worker: reliably publishes outbox messages
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IPublishEndpoint     _bus;
    private readonly ILogger<OutboxProcessor> _log;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishPendingAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task PublishPendingAsync(CancellationToken ct)
    {
        using var scope = _scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var pending = await db.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(100)
            .ToListAsync(ct);

        foreach (var msg in pending)
        {
            var type    = Type.GetType(msg.Type)!;
            var payload = JsonSerializer.Deserialize(msg.Payload, type)!;
            await _bus.Publish(payload, type, ct);
            msg.ProcessedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync(ct);
    }
}

#pragma warning restore
