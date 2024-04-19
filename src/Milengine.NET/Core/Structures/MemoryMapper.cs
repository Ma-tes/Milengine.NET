namespace Milengine.NET.Core.Structures;

public sealed class MemoryMapper<T>
    where T : class
{
    public Memory<T> Values { get; }
    public int MapIndex { get; set; }

    public MemoryMapper(int startIndex = 0, params T[] values)
    {
        Values = values;
        MapIndex = startIndex;
    }

    public MemoryMapper(Memory<T> values, int startIndex = 0)
    {
        Values = values;
        MapIndex = startIndex;
    }

    public ref T GetValueReference() =>
        ref Values.Span[MapIndex];
}

