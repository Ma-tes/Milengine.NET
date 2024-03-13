using System.Runtime.InteropServices;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics;

public enum FormatCountType : byte
{
    Vector1D = 1,
    Vector2D = 2,
    Vector3D = 3,
}

public enum VerticesType : byte
{
    Position = 0,
    Color = 1,
    Texture = 2,
}

public readonly struct FormatContainer
{
    public VerticesType VerticesType { get; }
    public FormatCountType Count { get; }
    public VertexAttribPointerType Type { get; }

    public FormatContainer(VerticesType verticesType, FormatCountType count, VertexAttribPointerType type)
    {
        VerticesType = verticesType;
        Count = count;
        Type = type;
    }
}

public struct MeshFormat : IGraphicsBindable
{
    public uint Handle { get; } = 0;
    public uint Stride { get; }

    public ReadOnlyMemory<FormatContainer> InlineFormatData { get; }

    public MeshFormat(ReadOnlyMemory<FormatContainer> inlineFormatData)
    {
        InlineFormatData = inlineFormatData;
        Stride = CalculateStride(InlineFormatData.Span);
    }

    public void Bind()
    {
        ReadOnlySpan<FormatContainer> inlineFormatDataSpan = InlineFormatData.Span;
        int inlineDataLength = inlineFormatDataSpan.Length;
        int relativeOffset = 0;
        for (int i = 0; i < inlineDataLength; i++)
        {
            FormatContainer currentContainer = inlineFormatDataSpan[i];
            GraphicsContext.SetVertexAttributePointer((uint)currentContainer.VerticesType,
                (int)currentContainer.Count,
                currentContainer.Type,
                Stride,
                relativeOffset
            );
            int vertexTypeSize = SizeOfVertexAttributeType(currentContainer.Type);
            relativeOffset += (int)currentContainer.Count * vertexTypeSize;
        }
    }

    private static uint CalculateStride(ReadOnlySpan<FormatContainer> inlineFormatData)
    {
        int returnStride = 0;
        int inlineDataLength = inlineFormatData.Length;
        for (int i = 0; i < inlineDataLength; i++)
        {
            FormatContainer currentContainer = inlineFormatData[i];
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
