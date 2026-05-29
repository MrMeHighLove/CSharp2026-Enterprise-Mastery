using CSharp2026.Common.Results;
using FluentAssertions;
using Xunit;

namespace CSharp2026.Common.Tests.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_Carries_Value()
    {
        var r = Result.Success(42);
        r.IsSuccess.Should().BeTrue();
        r.IsFailure.Should().BeFalse();
        r.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_Carries_Error()
    {
        var r = Result.Failure<int>("bad");
        r.IsFailure.Should().BeTrue();
        r.Error.Should().Be("bad");
    }

    [Fact]
    public void Match_Picks_Success_Branch()
    {
        var r = Result.Success(10);
        string s = r.Match(v => $"ok:{v}", e => $"err:{e}");
        s.Should().Be("ok:10");
    }

    [Fact]
    public void Match_Picks_Failure_Branch()
    {
        var r = Result.Failure<int>("nope");
        string s = r.Match(v => $"ok:{v}", e => $"err:{e}");
        s.Should().Be("err:nope");
    }

    [Fact]
    public void Map_Transforms_Success_Value()
    {
        var r = Result.Success(3).Map(x => x * 2);
        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(6);
    }

    [Fact]
    public void Map_Skips_Failure()
    {
        var r = Result.Failure<int>("bad").Map(x => x * 2);
        r.IsFailure.Should().BeTrue();
        r.Error.Should().Be("bad");
    }

    [Fact]
    public void Bind_Composes_Operations()
    {
        Result<int> Parse(string s) =>
            int.TryParse(s, out int n) ? Result.Success(n) : Result.Failure<int>($"'{s}' is not a number");

        var ok  = Result.Success("12").Bind(Parse);
        var bad = Result.Success("abc").Bind(Parse);

        ok.IsSuccess.Should().BeTrue();
        ok.Value.Should().Be(12);
        bad.IsFailure.Should().BeTrue();
        bad.Error.Should().Contain("'abc'");
    }
}
