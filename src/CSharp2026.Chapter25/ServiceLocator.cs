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

namespace CSharp2026.Chapter25;

#pragma warning disable
// Chapter25/ServiceLocator.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter25/ServiceLocator.cs
// ANTI-PATTERN: Service Locator — hides dependencies, impossible to test
public class OrderService
{
    public async Task ProcessAsync(Order order)
    {
        var validator = ServiceLocator.Resolve<IOrderValidator>(); // Hidden dep!
        var repo = ServiceLocator.Resolve<IOrderRepository>();     // Hidden dep!
        // ...
    }
}

// CORRECT: explicit constructor injection — all dependencies visible
public class OrderService
{
    private readonly IOrderValidator _validator;
    private readonly IOrderRepository _repo;

    public OrderService(IOrderValidator validator, IOrderRepository repo)
    {
        _validator = validator;
        _repo = repo;
    }

    public async Task ProcessAsync(Order order)
    {
        var result = await _validator.ValidateAsync(order);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        await _repo.SaveAsync(order);
    }
}

#pragma warning restore
