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
// Chapter04/OCP.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// OCP via strategy pattern + DI — adding new discount types without modifying existing code
public interface IDiscountStrategy
{
    decimal Apply(decimal price, CustomerTier tier);
}

public class NoDiscount : IDiscountStrategy
{
    public decimal Apply(decimal price, CustomerTier tier) => price;
}

public class VolumeDiscount : IDiscountStrategy
{
    private readonly decimal _threshold;
    private readonly decimal _rate;
    public VolumeDiscount(decimal threshold, decimal rate)
        => (_threshold, _rate) = (threshold, rate);

    public decimal Apply(decimal price, CustomerTier tier)
        => price > _threshold ? price * (1 - _rate) : price;
}

// New requirement: loyalty discount — add new class, zero modifications to existing code
public class LoyaltyDiscount : IDiscountStrategy
{
    public decimal Apply(decimal price, CustomerTier tier) =>
        tier switch
        {
            CustomerTier.Gold     => price * 0.85m,
            CustomerTier.Platinum => price * 0.75m,
            _                     => price,
        };
}

// Orchestrator never changes regardless of how many strategies we add
public class PricingEngine
{
    private readonly IEnumerable<IDiscountStrategy> _strategies;
    public PricingEngine(IEnumerable<IDiscountStrategy> strategies)
        => _strategies = strategies;

    public decimal FinalPrice(decimal basePrice, CustomerTier tier)
        => _strategies.Aggregate(basePrice, (p, s) => s.Apply(p, tier));
}

#pragma warning restore
