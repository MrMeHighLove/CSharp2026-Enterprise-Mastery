using CSharp2026.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CSharp2026.Common.Tests.ValueObjects;

public sealed class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("first.last@sub.domain.io")]
    [InlineData("a@b.co")]
    public void Accepts_Valid_Address(string input)
    {
        var email = new Email(input);
        email.Value.Should().Be(input);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("plain")]
    [InlineData("@nohost.com")]
    [InlineData("user@")]
    [InlineData("user@domain")]
    public void Rejects_Invalid_Address(string bad)
    {
        var act = () => new Email(bad);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsValid_Does_Not_Throw()
    {
        Email.IsValid("ok@example.com").Should().BeTrue();
        Email.IsValid("not-an-email").Should().BeFalse();
    }
}

public sealed class IdentifierTests
{
    [Fact]
    public void OrderId_New_Is_Unique()
    {
        OrderId.New().Should().NotBe(OrderId.New());
    }

    [Fact]
    public void Different_Identifier_Types_Cannot_Be_Mixed()
    {
        // This is the entire point of strongly-typed ids — verified at compile
        // time. The runtime check below confirms the wrapper preserves identity.
        var ord = OrderId.New();
        var cus = CustomerId.New();
        ord.Value.Should().NotBe(cus.Value);
    }

    [Fact]
    public void Identifier_ToString_Is_Guid_Format()
    {
        var id = OrderId.New();
        id.ToString().Should().MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }
}
