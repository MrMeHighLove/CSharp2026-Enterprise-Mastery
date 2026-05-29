using CSharp2026.Benchmarks;
using FluentAssertions;
using Xunit;

namespace CSharp2026.Benchmarks.Tests;

public sealed class BenchmarkCorrectnessTests
{
    [Fact]
    public void String_Processing_Benchmarks_Return_Equivalent_Output()
    {
        var benchmarks = new StringProcessingBenchmarks();
        string expected = benchmarks.StringConcat();

        benchmarks.StringBuilder().Should().Be(expected);
        benchmarks.StringCreate().Should().Be(expected);
        benchmarks.SpanProcessing().Should().Be(expected);
    }

    [Fact]
    public void Positive_Sum_Benchmarks_Return_Equivalent_Output()
    {
        var benchmarks = new LinqPositiveSumBenchmarks();
        int expected = benchmarks.LinqWhereSum();

        benchmarks.ForLoopSum().Should().Be(expected);
        benchmarks.SpanSum().Should().Be(expected);
    }
}
