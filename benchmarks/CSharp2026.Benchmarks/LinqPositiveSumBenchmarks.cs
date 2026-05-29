using BenchmarkDotNet.Attributes;

namespace CSharp2026.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class LinqPositiveSumBenchmarks
{
    private readonly int[] _numbers = Enumerable.Range(-5_000, 10_000).ToArray();

    [Benchmark(Baseline = true)]
    public int LinqWhereSum() => _numbers.Where(static number => number > 0).Sum();

    [Benchmark]
    public int ForLoopSum()
    {
        int sum = 0;

        for (int index = 0; index < _numbers.Length; index++)
        {
            int number = _numbers[index];
            if (number > 0)
            {
                sum += number;
            }
        }

        return sum;
    }

    [Benchmark]
    public int SpanSum()
    {
        int sum = 0;
        ReadOnlySpan<int> span = _numbers;

        foreach (int number in span)
        {
            if (number > 0)
            {
                sum += number;
            }
        }

        return sum;
    }
}
