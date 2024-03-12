using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;

namespace Milengine.NET.Core.Utilities;

public static class SpanHelper<T>
    where T : notnull
{
    public static Span<T> CreateFixedParameterSpan<TBuffer>(TBuffer buffer)
        where TBuffer : IInlineIndexParameter
    {
        int bufferSize = TBuffer.Length;
        return MemoryMarshal.CreateSpan(ref Unsafe.As<TBuffer, T>(ref buffer), bufferSize);
    }

    public static Span<T> CreateFixedParameterSpanT<TBuffer>(TBuffer buffer)
        where TBuffer : IInlineIndexParameter<T>
    {
        int bufferSize = TBuffer.Length;
        return MemoryMarshal.CreateSpan(ref Unsafe.As<TBuffer, T>(ref buffer), bufferSize);
    }

    public static ReadOnlySpan<T> CreateFixedParameterReadOnlySpan<TBuffer>(TBuffer buffer)
        where TBuffer : IInlineIndexParameter
    {
        int bufferSize = TBuffer.Length;
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TBuffer, T>(ref buffer), bufferSize);
    }

    public static ReadOnlySpan<T> CreateFixedParameterReadOnlySpanT<TBuffer>(TBuffer buffer)
        where TBuffer : IInlineIndexParameter<T>
    {
        int bufferSize = TBuffer.Length;
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TBuffer, T>(ref buffer), bufferSize);
    }
}