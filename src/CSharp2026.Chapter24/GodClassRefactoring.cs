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

namespace CSharp2026.Chapter24;

#pragma warning disable
// Chapter24/GodClassRefactoring.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter24/GodClassRefactoring.cs
// BEFORE: LegacyOrderManager has 80 methods, 2000 lines
public class LegacyOrderManager
{
    // Pricing logic — 20 methods
    public decimal CalculatePrice(Order order) { /* ... */ }
    public decimal ApplyDiscount(Order order, Discount d) { /* ... */ }

    // Validation logic — 15 methods
    public bool ValidateOrder(Order order) { /* ... */ }
    public IList<string> GetValidationErrors(Order order) { /* ... */ }

    // Fulfillment logic — 25 methods
    public void Reserve(Order order) { /* ... */ }
    public void Ship(Order order) { /* ... */ }
}

// AFTER: Extract by cohesion — each class has a single responsibility
public class OrderPricingService
{
    public decimal Calculate(Order order) { /* ... */ }
    public decimal ApplyDiscount(Order order, Discount d) { /* ... */ }
}

public class OrderValidator
{
    public ValidationResult Validate(Order order) { /* ... */ }
}

public class FulfillmentService
{
    public async Task ReserveAsync(Order order) { /* ... */ }
    public async Task ShipAsync(Order order) { /* ... */ }
}

// Façade to maintain backward compatibility during migration
public class LegacyOrderManager
{
    private readonly OrderPricingService _pricing;
    private readonly OrderValidator _validator;
    private readonly FulfillmentService _fulfillment;

    public decimal CalculatePrice(Order order) => _pricing.Calculate(order);
    public bool ValidateOrder(Order order) => _validator.Validate(order).IsValid;
    public void Reserve(Order order) => _fulfillment.ReserveAsync(order).Wait();
}

#pragma warning restore
