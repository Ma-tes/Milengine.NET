using System.Numerics;
using System.Runtime.InteropServices;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics.Structures;

public struct GraphicsBufferData<T> : IGraphicsBindable
    where T : notnull
{
    public uint Handle { get; }
    public BufferTargetARB BufferTarget { get; }

    public GraphicsBufferData(ReadOnlySpan<T> data, BufferTargetARB bufferTarget)
    {
        BufferTarget = bufferTarget;
        Handle = GraphicsContext.Graphics.GenBuffer();


    }

    public void Bind() => GraphicsContext.Graphics.BindBuffer(BufferTarget, Handle);

    private void CreateRealtiveBuffer(ReadOnlySpan<T> data)
    {
        int bufferSize = data.Length * Marshal.SizeOf(default(T));
        GraphicsContext.Graphics.BufferData<T>(BufferTarget, data, BufferUsageARB.StaticDraw);
    }

    public void Dispose() {}
}
