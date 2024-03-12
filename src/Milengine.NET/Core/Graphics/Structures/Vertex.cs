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
    public Vector2D<T> TextureCoordinates { get; }

    public Vertex(Vector3D<T> position,
        Vector3D<T> colorNormals, Vector2D<T> textureCoordinates)
    {
        Position = position;
        ColorNormals = colorNormals;
        TextureCoordinates = textureCoordinates;
    }

    //TODO: Find a related way of unsafe casting generics
    //inline array.
    public static ReadOnlySpan<T> CombineInlineVertexData(Vertex<T> vertex)
    {
        InlineParameter_Eight<T> returnParameterVertex =
            InlineValueParameter_Eight<T>.CreateInstance(
                vertex.Position.X, vertex.Position.Y, vertex.Position.Z,
                vertex.ColorNormals.X, vertex.ColorNormals.Y, vertex.ColorNormals.Z,
                vertex.TextureCoordinates.X, vertex.TextureCoordinates.Y);
        return SpanHelper<T>.CreateFixedParameterReadOnlySpan(returnParameterVertex);
    }

    public static ReadOnlySpan<T> CombineVertexData(Vertex<T> vertex) =>
        new ReadOnlySpan<T>([
            vertex.Position.X, vertex.Position.Y, vertex.Position.Z,
            vertex.ColorNormals.X, vertex.ColorNormals.Y, vertex.ColorNormals.Z,
            vertex.TextureCoordinates.X, vertex.TextureCoordinates.Y
        ]);

    //TODO: Find a related way of unsafe casting generics
    //inline array.
    public static ReadOnlyMemory<T> CreateMultipleInlineVertexCombination(ReadOnlySpan<Vertex<T>> vertexCombination)
    {
        int vertexCombinationLength = vertexCombination.Length;
        Memory<T> resultVertexCombination = new Memory<T>(new T[8 * vertexCombinationLength]);
        Span<T> resultVertexCombinationSpan = resultVertexCombination.Span;
        for (int i = 0; i < vertexCombinationLength; i++)
        {
            ReadOnlySpan<T> relativeVertexData = CombineInlineVertexData(vertexCombination[i]);
            relativeVertexData.CopyTo(resultVertexCombinationSpan[(i * 8)..]);
        }
        return resultVertexCombination;
    }

    public static ReadOnlyMemory<T> CreateMultipleVertexCombination(ReadOnlySpan<Vertex<T>> vertexCombination)
    {
        int vertexCombinationLength = vertexCombination.Length;
        Memory<T> resultVertexCombination = new Memory<T>(new T[8 * vertexCombinationLength]);
        Span<T> resultVertexCombinationSpan = resultVertexCombination.Span;
        for (int i = 0; i < vertexCombinationLength; i++)
        {
            ReadOnlySpan<T> relativeVertexData = CombineVertexData(vertexCombination[i]);
            relativeVertexData.CopyTo(resultVertexCombinationSpan[(i * 8)..]);
        }
        return resultVertexCombination;
    }
}