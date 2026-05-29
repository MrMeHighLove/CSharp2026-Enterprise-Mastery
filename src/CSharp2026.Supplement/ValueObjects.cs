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

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/ValueObjects.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ValueObjects.cs
// Domain primitive: Email — impossible to pass an invalid email
public readonly record struct Email
{
    private static readonly Regex _pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Result.Failure<Email>("Email cannot be empty");
        var normalised = raw.Trim().ToLowerInvariant();
        if (!_pattern.IsMatch(normalised))
            return Result.Failure<Email>($"'{raw}' is not a valid email");
        return Result.Success(new Email(normalised));
    }

    public static implicit operator string(Email e) => e.Value;
    public override string ToString() => Value;
}

// Typed ID: prevents passing the wrong ID to the wrong method
public readonly record struct OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
    public static OrderId Parse(string s) => new(Guid.Parse(s));
    public override string ToString() => Value.ToString();
}

// Money: encapsulates currency and arithmetic rules
public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot add {Currency} and {other.Currency}");
        return this with { Amount = Amount + other.Amount };
    }

    public Money Multiply(decimal factor) => this with { Amount = Amount * factor };
    public Money ApplyDiscount(decimal pct) => Multiply(1 - pct / 100);
}

// Now your Order is self-documenting and safe:
public class Order
{
    public OrderId Id { get; } = OrderId.New();
    public Email CustomerEmail { get; }   // Cannot be invalid
    public Money Total { get; private set; } // Cannot mix currencies
}

#pragma warning restore
