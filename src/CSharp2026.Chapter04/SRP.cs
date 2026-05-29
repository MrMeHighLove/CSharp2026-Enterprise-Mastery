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
// Chapter04/SRP.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Multiple responsibilities: business logic + data access + email
public class OrderService
{
    private readonly SqlConnection _connection;
    private readonly SmtpClient _smtp;

    public void PlaceOrder(OrderRequest req)
    {
        // Business logic
        if (req.Total > 10000)
            throw new BusinessException("Orders over $10,000 require approval");

        // Data access
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Orders ...";
        cmd.ExecuteNonQuery();

        // Email
        _smtp.Send(new MailMessage("no-reply@co.com", req.Email, "Order Confirmed", "..."));
    }
}

// GOOD: Three focused classes, each with one reason to change
public class OrderValidationService
{
    public void Validate(OrderRequest req)
    {
        if (req.Total > 10000)
            throw new BusinessException("Orders over $10,000 require approval");
    }
}

public class OrderRepository
{
    private readonly AppDbContext _db;
    public OrderRepository(AppDbContext db) => _db = db;
    public async Task SaveAsync(Order order, CancellationToken ct)
        => await _db.Orders.AddAsync(order, ct);
}

public class OrderNotificationService
{
    private readonly IEmailSender _email;
    public OrderNotificationService(IEmailSender email) => _email = email;
    public Task SendConfirmationAsync(Order order, CancellationToken ct)
        => _email.SendAsync(new OrderConfirmationEmail(order), ct);
}

#pragma warning restore
