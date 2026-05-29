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

namespace CSharp2026.Chapter04;

#pragma warning disable
// Chapter04/ISP.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Fat interface — ReadOnlyReportService has to implement mutations it doesn't use
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct);
    Task<IReadOnlyList<Order>> GetByCustomerAsync(CustomerId id, CancellationToken ct);
    Task SaveAsync(Order order, CancellationToken ct);
    Task DeleteAsync(OrderId id, CancellationToken ct);
    Task<IReadOnlyList<Order>> SearchAsync(OrderSearchQuery q, CancellationToken ct);
}

// GOOD: Segregated interfaces — each client depends only on what it uses
public interface IOrderReader
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct);
    Task<IReadOnlyList<Order>> GetByCustomerAsync(CustomerId id, CancellationToken ct);
    Task<IReadOnlyList<Order>> SearchAsync(OrderSearchQuery q, CancellationToken ct);
}

public interface IOrderWriter
{
    Task SaveAsync(Order order, CancellationToken ct);
    Task DeleteAsync(OrderId id, CancellationToken ct);
}

// Implementation composes both
public class OrderRepository : IOrderReader, IOrderWriter { ... }

// Report service depends only on reading
public class OrderReportService
{
    private readonly IOrderReader _reader;
    public OrderReportService(IOrderReader reader) => _reader = reader;
    // No access to Save/Delete — impossible to introduce accidental mutations
}

#pragma warning restore
