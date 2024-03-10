using BenchmarkDotNet.Running;
using Milengine.NET.Benchmark;

var spanAccessBenchmark = BenchmarkRunner.Run<SpanAccessBenchmark>();