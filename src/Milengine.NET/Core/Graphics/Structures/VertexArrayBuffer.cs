using Milengine.NET.Core.Graphics.Interfaces;

namespace Milengine.NET.Core.Graphics.Structures;

public struct VertexArrayBuffer<TVbo, TEbo> : IGraphicsBindable
    where TVbo : unmanaged
    where TEbo : unmanaged
{
    public uint Handle { get; }

    public VertexArrayBuffer(
        GraphicsBufferData<TVbo> vertexBuffer,
        GraphicsBufferData<TEbo> indexBuffer)
    {
        Handle = GraphicsContext.Graphics.GenVertexArray(); Bind();
        vertexBuffer.Bind();
        indexBuffer.Bind();
    }

    public void Bind() => GraphicsContext.Graphics.BindVertexArray(Handle);

    public void Dispose() { GraphicsContext.Graphics.DeleteBuffer(Handle); }
}
