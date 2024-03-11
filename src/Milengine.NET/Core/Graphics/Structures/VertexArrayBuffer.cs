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

    public void SetVertexAttributePointer(uint index, int typeCount,
        VertexAttribPointerType type, uint vertexSize, int offset)
    {
        uint relativeVertexSize = vertexSize * (uint)Marshal.SizeOf(default(TVbo));
        int relativeOffset = offset * Marshal.SizeOf(default(TVbo));

        unsafe { GraphicsContext.Graphics.VertexAttribPointer(index,
            typeCount, type, false, relativeVertexSize, (void*)relativeOffset); }
        GraphicsContext.Graphics.EnableVertexAttribArray(index);
    }

    public void Dispose() { GraphicsContext.Graphics.DeleteBuffer(Handle); }
}
