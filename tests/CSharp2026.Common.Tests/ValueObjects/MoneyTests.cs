using CSharp2026.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CSharp2026.Common.Tests.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Zero_Has_Amount_Zero_Usd()
    {
        Money.Zero.Amount.Should().Be(0m);
        Money.Zero.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData("usd", "USD")]
    [InlineData("eur", "EUR")]
    [InlineData("Gbp", "GBP")]
    public void Currency_Is_Uppercased(string input, string expected)
    {
        var m = new Money(1m, input);
        m.Currency.Should().Be(expected);
    }

    [Fact]
    public void Empty_Currency_Throws()
    {
        var act = () => new Money(1m, "");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("US")]
    [InlineData("DOLLARS")]
    public void Non_Three_Letter_Currency_Throws(string bad)
    {
        var act = () => new Money(1m, bad);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*3-letter*");
    }

    [Fact]
    public void Adds_Same_Currency()
    {
        var a = new Money(10m, "USD");
        var b = new Money(5m,  "USD");
        (a + b).Should().Be(new Money(15m, "USD"));
    }

    [Fact]
    public void Subtracts_Same_Currency()
    {
        var a = new Money(10m, "EUR");
        var b = new Money(3m,  "EUR");
        (a - b).Should().Be(new Money(7m, "EUR"));
    }

    [Fact]
    public void Multiplies_By_Scalar()
    {
        var a = new Money(2.5m, "USD");
        (a * 4m).Should().Be(new Money(10m, "USD"));
    }

    [Fact]
    public void Mixed_Currency_Add_Throws()
    {
        var a = new Money(10m, "USD");
        var b = new Money(10m, "EUR");
        var act = () => a + b;
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*USD*EUR*");
    }

    [Fact]
    public void Equality_Is_Structural()
    {
        var a = new Money(10m, "USD");
        var b = new Money(10m, "USD");
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void ToString_Is_Culture_Invariant()
    {
        var m = new Money(1234.5m, "USD");
        m.ToString().Should().Be("1234.50 USD");
    }
}
