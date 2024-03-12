using System.Runtime.InteropServices;
using Milengine.NET.Core.Graphics.Interfaces;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics;

public enum FormatCountType : byte
{
    Vector1D = 1,
    Vector2D = 2,
    Vector3D = 3,
}

public readonly struct FormatContainer
{
    public uint Index { get; }
    public FormatCountType Count { get; }
    public VertexAttribPointerType Type { get; }

    public FormatContainer(uint index, FormatCountType count, VertexAttribPointerType type)
    {
        Index = index;
        Count = count;
        Type = type;
    }
}

public sealed class MeshFormat<T> : IGraphicsBindable
    where T : IInlineIndexParameter<FormatContainer>
{
    public uint Handle { get; } = 0;
    public uint Stride { get; }

    public T InlineFormatData { get; }

    public MeshFormat(T inlineFormatData)
    {
        InlineFormatData = inlineFormatData;
        Stride = CalculateStride(InlineFormatData);
    }

    public void Bind()
    {
        int inlineDataLength = T.Length;
        int relativeOffset = 0;
        for (int i = 0; i < inlineDataLength; i++)
        {
            FormatContainer currentContainer = InlineFormatData.GetNonDirectInlineParameter(i);
            GraphicsContext.SetVertexAttributePointer(currentContainer.Index,
                (int)currentContainer.Count,
                currentContainer.Type,
                Stride,
                relativeOffset
            );
            int vertexTypeSize = SizeOfVertexAttributeType(currentContainer.Type);
            relativeOffset += (int)currentContainer.Count * vertexTypeSize;
        }
    }

    private static uint CalculateStride(T inlineFormatData)
    {
        int returnStride = 0;
        int inlineDataLength = T.Length;
        for (int i = 0; i < inlineDataLength; i++)
        {
            FormatContainer currentContainer = inlineFormatData.GetNonDirectInlineParameter(i);
            returnStride += (int)currentContainer.Count * SizeOfVertexAttributeType(currentContainer.Type);
        }
        return (uint)returnStride;
    }

    private static int SizeOfVertexAttributeType(VertexAttribPointerType type) => type switch
    {
        VertexAttribPointerType.Float => Marshal.SizeOf(typeof(float)),
        VertexAttribPointerType.Short => Marshal.SizeOf(typeof(short)),
        VertexAttribPointerType.Byte => Marshal.SizeOf(typeof(byte)),
        _ => 0
    };

    public void Dispose() { }
}
