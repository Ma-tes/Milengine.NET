using Milengine.NET.Core.Graphics;

namespace Milengine.NET.Core.SceneManager;

public struct TickCounter
{
    private double initialStartTick = 0;
    private int framesCounter = 0;

    public long CurrentTick { get; private set; }
    public long DeltaTimeTick { get; set; }

    public TickCounter(long deltaTimeTick)
    {
        DeltaTimeTick = deltaTimeTick;
    }

    public double CalculateRelativeTimeTick() =>
        (GraphicsContext.Global.Window.Time - initialStartTick) / DeltaTimeTick;

    public int CalculateRelativeFramesPerSecond()
    {
        initialStartTick = initialStartTick == 0 ? GraphicsContext.Global.Window.Time : initialStartTick;
        double currentTimeTickDifference = CalculateRelativeTimeTick();
        if(currentTimeTickDifference > 1000.0f)
        {
            framesCounter = 0;
            initialStartTick = GraphicsContext.Global.Window.Time;
        }
        framesCounter++;
        return framesCounter;
    }
} 