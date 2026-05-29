using CSharp2026.Chapter05.Builder;
using FluentAssertions;
using Xunit;
using BookHttpRequestOptions = CSharp2026.Chapter05.Builder.HttpRequestOptions;

namespace CSharp2026.Chapter05.Tests.Builder;

public sealed class BuilderPatternTests
{
    [Fact]
    public void Default_Build_Throws_When_BaseUri_Missing()
    {
        var act = () => BookHttpRequestOptions.Builder().Build();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*BaseUri*");
    }

    [Fact]
    public void Build_Captures_All_Properties()
    {
        var options = BookHttpRequestOptions.Builder()
            .WithBaseUri(new Uri("https://api.example.com"))
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithMaxRetries(5)
            .WithFollowRedirects(false)
            .WithHeader("X-Tenant", "acme")
            .Build();

        options.BaseUri.Should().Be(new Uri("https://api.example.com"));
        options.Timeout.Should().Be(TimeSpan.FromSeconds(10));
        options.MaxRetries.Should().Be(5);
        options.FollowRedirects.Should().BeFalse();
        options.Headers["X-Tenant"].Should().Be("acme");
    }

    [Fact]
    public void Default_Headers_Are_Empty_Read_Only_Dictionary()
    {
        // This is the exact compile bug the reviewer caught:
        // ReadOnlyDictionary<,>.Empty does not exist. Verify the
        // explicit-constructor fix produces a usable empty read-only map.
        var options = BookHttpRequestOptions.Builder()
            .WithBaseUri(new Uri("https://x"))
            .Build();

        options.Headers.Should().NotBeNull();
        options.Headers.Should().BeEmpty();
        // Underlying type is ReadOnlyDictionary; mutating it is not possible.
        ((object)options.Headers).Should().BeAssignableTo<IReadOnlyDictionary<string, string>>();
    }

    [Fact]
    public void Header_Keys_Are_Case_Insensitive()
    {
        var options = BookHttpRequestOptions.Builder()
            .WithBaseUri(new Uri("https://x"))
            .WithHeader("Content-Type", "application/json")
            .Build();

        options.Headers["content-type"].Should().Be("application/json");
        options.Headers["CONTENT-TYPE"].Should().Be("application/json");
    }

    [Fact]
    public void Repeated_Header_Last_Write_Wins()
    {
        var options = BookHttpRequestOptions.Builder()
            .WithBaseUri(new Uri("https://x"))
            .WithHeader("X-Trace-Id", "a")
            .WithHeader("X-Trace-Id", "b")
            .Build();

        options.Headers["X-Trace-Id"].Should().Be("b");
    }

    [Fact]
    public void Negative_Retries_Rejected_At_Builder_Time()
    {
        var act = () => BookHttpRequestOptions.Builder().WithMaxRetries(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Built_Options_Are_Immutable_Snapshots()
    {
        // Senior-architect property: subsequent builder mutations must not
        // leak into already-built instances.
        var b = BookHttpRequestOptions.Builder().WithBaseUri(new Uri("https://x"));
        var first = b.WithHeader("k", "1").Build();
        b.WithHeader("k", "2");                    // mutate builder after Build()

        first.Headers["k"].Should().Be("1");        // first snapshot unchanged
    }
}
