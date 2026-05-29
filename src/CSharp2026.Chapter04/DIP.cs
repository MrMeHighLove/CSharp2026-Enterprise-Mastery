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
// Chapter04/DIP.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: High-level module hard-wires to a low-level implementation
public class OrderProcessingService
{
    private readonly SqlOrderRepository _repo = new SqlOrderRepository(
        connectionString: "Server=proddb01;Database=Orders;...");
    private readonly SmtpEmailService _email = new SmtpEmailService(host: "mail.company.com");

    public async Task ProcessAsync(OrderRequest req, CancellationToken ct)
    {
        // Cannot test without a real SQL Server and an SMTP server
        var order = Order.Create(req);
        await _repo.SaveAsync(order, ct);
        await _email.SendConfirmationAsync(order, ct);
    }
}

// GOOD: Depending on abstractions — fully testable, extensible
public class OrderProcessingService
{
    private readonly IOrderWriter _orders;
    private readonly IOrderNotifier _notifier;

    public OrderProcessingService(IOrderWriter orders, IOrderNotifier notifier)
        => (_orders, _notifier) = (orders, notifier);

    public async Task ProcessAsync(OrderRequest req, CancellationToken ct)
    {
        var order = Order.Create(req);
        await _orders.SaveAsync(order, ct);
        await _notifier.NotifyAsync(order, ct);
    }
}

// DI container wires the real implementations in production:
builder.Services.AddScoped<IOrderWriter, SqlOrderRepository>();
builder.Services.AddScoped<IOrderNotifier, EmailOrderNotifier>();
// Test replaces them with fakes — zero code change in the service

#pragma warning restore
