using System.Numerics;
using System.Runtime.InteropServices;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics.Structures;

public struct GraphicsBufferData<T> : IGraphicsBindable
    where T : unmanaged, INumber<T>
{
    public uint Handle { get; } = GraphicsContext.Graphics.GenBuffer();
    public BufferTargetARB BufferTarget { get; }
    public ReadOnlyMemory<T> Buffer { get; }

    public GraphicsBufferData(ReadOnlyMemory<T> data, BufferTargetARB bufferTarget)
    {
        BufferTarget = bufferTarget;
        Buffer = data;

        Bind();
        CreateRelativeBuffer(data.Span);
    }

    public void Bind() => GraphicsContext.Graphics.BindBuffer(BufferTarget, Handle);

    private void CreateRelativeBuffer(ReadOnlySpan<T> data) =>
        GraphicsContext.Graphics.BufferData(BufferTarget, data, BufferUsageARB.StaticDraw);

    public void Dispose() { GraphicsContext.Graphics.DeleteBuffer(Handle); }
}
