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

namespace CSharp2026.Chapter20;

#pragma warning disable
// Chapter20/InputValidation.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// SQL injection prevention: parameterised queries ALWAYS (EF Core does this by default)
// Direct string interpolation in EF Core FromSqlRaw is dangerous:

// AVOID: SQL INJECTION VULNERABILITY
var search = userInput; // "'; DROP TABLE Orders;--"
var orders = _db.Orders.FromSqlRaw($"SELECT * FROM Orders WHERE CustomerId = '{search}'");

// GOOD: Parameterised — safe against SQL injection
var orders = _db.Orders.FromSqlRaw(
    "SELECT * FROM Orders WHERE CustomerId = {0}", customerId.ToString());

// Or better: use LINQ which is always parameterised
var orders = _db.Orders.Where(o => o.CustomerId == customerId);

// Input validation at the API boundary
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(r => r.CustomerId)
            .NotEmpty()
            .Must(BeAValidGuid).WithMessage("CustomerId must be a valid GUID.");

        RuleFor(r => r.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .Must(items => items.Count <= 100).WithMessage("Maximum 100 items per order.");

        RuleForEach(r => r.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity).InclusiveBetween(1, 9999);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0m);
        });
    }
    private static bool BeAValidGuid(string value) => Guid.TryParse(value, out _);
}

#pragma warning restore
