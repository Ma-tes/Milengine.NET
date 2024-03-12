using System.Numerics;
using Milengine.NET.Core.Utilities;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Graphics.Structures;

public readonly struct Vertex<T>
    where T : unmanaged, INumber<T>
{
    public Vector3D<T> Position { get; }
    public Vector3D<T> ColorNormals { get; }
    public Vector3D<T> TextureCoordinates { get; }

    public Vertex(Vector3D<T> position,
        Vector3D<T> colorNormals, Vector3D<T> textureCoordinates)
    {
        Position = position;
        ColorNormals = colorNormals;
        TextureCoordinates = textureCoordinates;
    }

    public static ReadOnlySpan<T> CombineVertexData(Vertex<T> vertex)
    {
        InlineParameter_Nine<T> returnParameterVertex =
            InlineValueParameter_Nine<T>.CreateInstance(
                vertex.Position.X, vertex.Position.Y, vertex.Position.Z,
                vertex.ColorNormals.X, vertex.ColorNormals.Y, vertex.ColorNormals.Z,
                vertex.TextureCoordinates.X, vertex.TextureCoordinates.Y, vertex.TextureCoordinates.Z
            );
        return SpanHelper<T>.CreateFixedParameterReadOnlySpanT(returnParameterVertex);
    }
}
