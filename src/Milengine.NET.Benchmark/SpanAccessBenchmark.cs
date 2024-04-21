using BenchmarkDotNet.Attributes;
using Milengine.NET.Core.Utilities;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;

namespace Milengine.NET.Benchmark;

[MemoryDiagnoser]
public class SpanAccessBenchmark
{
    [Params(128, 2048, 100000, 1000000)]
    public int Interation { get; set; }

    [Benchmark]
    public void CreateFloatSumArray()
    {
        float[] iterationSumResult = new float[Interation * 3];
        for (int i = 0; i < Interation; i++)
        {
            float[] currentSpan = [i * 1, i * 2, i * 3];
            for (int j = 0; j < currentSpan.Length; j++)
            {
                int relativeIndex = (currentSpan.Length * i) + j;
                iterationSumResult[relativeIndex] = currentSpan[j];
            }
        }
    }

    [Benchmark]
    public void CreateFloatSumFixedArray()
    {
        float[] iterationSumResult = new float[Interation * 3];
        for (int i = 0; i < Interation; i++)
        {
            var fixedInlineValue = InlineValueParameter_Three<float>.CreateInstance(i * 1, i * 2, i * 3);
            for (int j = 0; j < InlineParameter_Three<float>.Length; j++)
            {
                int relativeIndex = (InlineParameter_Three<float>.Length * i) + j;
                iterationSumResult[relativeIndex] = fixedInlineValue[j];
            }
        }
    }
}
