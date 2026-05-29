using BenchmarkDotNet.Attributes;
using System.Text;

namespace CSharp2026.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class StringProcessingBenchmarks
{
    private readonly string _input = "The quick brown fox jumps over the lazy dog";

    [Benchmark(Baseline = true)]
    public string StringConcat()
    {
        string result = string.Empty;
        foreach (string word in _input.Split(' '))
        {
            result += word.ToUpperInvariant() + " ";
        }

        return result.TrimEnd();
    }

    [Benchmark]
    public string StringBuilder()
    {
        var builder = new StringBuilder();
        foreach (string word in _input.Split(' '))
        {
            builder.Append(word.ToUpperInvariant()).Append(' ');
        }

        if (builder.Length > 0)
        {
            builder.Length--;
        }

        return builder.ToString();
    }

    [Benchmark]
    public string StringCreate()
    {
        string[] words = _input.Split(' ');
        return string.Create(_input.Length, words, static (span, state) =>
        {
            int position = 0;
            foreach (string word in state)
            {
                word.ToUpperInvariant().AsSpan().CopyTo(span[position..]);
                position += word.Length;

                if (position < span.Length)
                {
                    span[position++] = ' ';
                }
            }
        });
    }

    [Benchmark]
    public string SpanProcessing()
    {
        Span<char> buffer = stackalloc char[_input.Length];
        _input.AsSpan().CopyTo(buffer);

        for (int index = 0; index < buffer.Length; index++)
        {
            if (buffer[index] != ' ')
            {
                buffer[index] = char.ToUpperInvariant(buffer[index]);
            }
        }

        return buffer.ToString();
    }
}
