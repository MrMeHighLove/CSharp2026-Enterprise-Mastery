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

namespace CSharp2026.Chapter23;

#pragma warning disable
// Chapter23/PrimaryConstructors.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter23/PrimaryConstructors.cs

// GOOD: simple service — primary constructor shines here
public class OrderValidator(ILogger<OrderValidator> logger, IOptions<OrderRules> options)
{
    private readonly OrderRules _rules = options.Value;

    public ValidationResult Validate(Order order)
    {
        logger.LogDebug("Validating order {Id}", order.Id);
        return order.Amount > _rules.MaxAmount
            ? ValidationResult.Fail("Amount exceeds limit")
            : ValidationResult.Ok();
    }
}

// BAD: complex domain class — primary constructor parameters leak everywhere
// Avoid: the 'config' parameter is accessible in all 200 lines of this class
// making it hard to reason about what actually uses it
public class ComplexDomainService(AppConfig config, IRepository repo, ICache cache,
    IEventBus bus, ILogger<ComplexDomainService> logger)
{
    // 200 lines of complex logic...
    // 'config' is accessible in all of them, which is confusing
}

// BETTER for complex classes: explicit field assignments remain clearer
public class ComplexDomainService
{
    private readonly AppConfig _config;
    private readonly IRepository _repo;
    // ... explicit fields make it obvious what this class actually uses
}

#pragma warning restore
