// -----------------------------------------------------------------------
//   Money.cs
//   Value object: a non-negative monetary amount with an ISO currency.
//   Records give us structural equality and immutability for free.
// -----------------------------------------------------------------------

using System.Globalization;

namespace CSharp2026.Common.ValueObjects;

/// <summary>
/// A monetary amount in a single currency. Two <see cref="Money"/> values are
/// equal when their amount and currency are equal. Arithmetic operators
/// refuse cross-currency operations rather than silently producing wrong
/// totals — fail loudly is the senior-engineering default for money.
/// </summary>
public readonly record struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public static Money Zero { get; } = new(0m, "USD");

    public Money(decimal amount, string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        if (currency.Length != 3)
        {
            throw new ArgumentException(
                "Currency must be a 3-letter ISO 4217 code (e.g. USD, EUR, GBP).",
                nameof(currency));
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    public static Money operator +(Money a, Money b) => a.Add(b);
    public static Money operator -(Money a, Money b) => a.Subtract(b);
    public static Money operator *(Money a, decimal factor) => a.Multiply(factor);

    public override string ToString() =>
        $"{Amount.ToString("0.00", CultureInfo.InvariantCulture)} {Currency}";

    private void EnsureSameCurrency(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Cannot combine money in different currencies: {Currency} and {other.Currency}.");
        }
    }
}
