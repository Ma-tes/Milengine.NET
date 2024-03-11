using System.Diagnostics;

namespace Milengine.NET.Core.SceneManager;

public struct TickCounter
{
    private readonly long initialStartTick = 0;

    public long CurrentTick { get; private set;}
    public long DeltaTimeTick { get; set; }

    public TickCounter(long deltaTimeTick = 1)
    {
        DeltaTimeTick = deltaTimeTick;
        initialStartTick = Stopwatch.GetTimestamp();
    }

    public float CalculateRelativeTimeTick() =>
        (Stopwatch.GetTimestamp() - initialStartTick) / DeltaTimeTick;
}
