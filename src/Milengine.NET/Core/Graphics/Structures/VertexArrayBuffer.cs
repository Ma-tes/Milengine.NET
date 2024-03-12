using System.Numerics;
using System.Runtime.InteropServices;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics.Structures;

public struct VertexArrayBuffer<TVbo, TEbo> : IGraphicsBindable
    where TVbo : unmanaged, INumber<TVbo>
    where TEbo : unmanaged, INumber<TEbo>
{
    public uint Handle { get; } = GraphicsContext.Graphics.GenVertexArray();

    public VertexArrayBuffer(
        GraphicsBufferData<TVbo> vertexBuffer,
        GraphicsBufferData<TEbo> indexBuffer)
    {
        Bind();
        vertexBuffer.Bind();
        indexBuffer.Bind();
    }

    public void Bind() => GraphicsContext.Graphics.BindVertexArray(Handle);


    public void Dispose() { GraphicsContext.Graphics.DeleteBuffer(Handle); }
}
