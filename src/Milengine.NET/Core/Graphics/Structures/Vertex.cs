using System.Numerics;
using Milengine.NET.Core.Graphics.Enums;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Graphics.Structures;

public struct Vertex<T>
    where T : unmanaged, INumber<T>
{
    public Vector3D<T> Position { get; set; }
    public Vector3D<T> ColorNormals { get; set; }
    public Vector2D<T> TextureCoordinates { get; set; }

    public static Vertex<T> Zero { get; } =
        new Vertex<T>(Vector3D<T>.Zero, Vector3D<T>.Zero, Vector2D<T>.Zero);

    public Vertex() { }
    public Vertex(Vector3D<T> position,
        Vector3D<T> colorNormals, Vector2D<T> textureCoordinates)
    {
        Position = position;
        ColorNormals = colorNormals;
        TextureCoordinates = textureCoordinates;
    }

    public Vertex(Vector3D<T> position,
        Vector3D<T> colorNormals)
    {
        Position = position;
        ColorNormals = colorNormals;
        TextureCoordinates = Vector2D<T>.Zero;
    }

    public static Memory<T> CombineVertexData(Vertex<T> vertex,
        ReadOnlyMemory<FormatContainer> formatContainer)
    {
        Memory<T> vertexData = new Memory<T>(new T[8]);
        Span<T> vertexDataSpan = vertexData.Span;

        ReadOnlySpan<FormatContainer> formatContainerSpan = formatContainer.Span;
        int vertexDataCount = 0;
        for (int i = 0; i < formatContainerSpan.Length; i++)
        {
            FormatContainer currentContainer = formatContainerSpan[i];
            Vector3D<T> relativeVertexData = GetRelativeVertexVector(vertex, currentContainer.VerticesType);

            int currentContainerTypeCount = (int)currentContainer.Count;
            for (int j = 0; j < currentContainerTypeCount; j++) { vertexDataSpan[vertexDataCount + j] = relativeVertexData[j]; }
            vertexDataCount += currentContainerTypeCount;
        }
        return vertexData;
    }

    public static ReadOnlyMemory<T> CreateMultipleVertexCombination(ReadOnlySpan<Vertex<T>> vertexCombination,
        ReadOnlyMemory<FormatContainer> formatContainer)
    {
        int vertexCombinationLength = vertexCombination.Length;
        Memory<T> resultVertexCombination = new Memory<T>(new T[8 * vertexCombinationLength]);
        Span<T> resultVertexCombinationSpan = resultVertexCombination.Span;

        int vertexDataLength = 0;
        for (int i = 0; i < vertexCombinationLength; i++)
        {
            if(!vertexCombination[i].Equals(Zero))
            {
                Span<T> combineVertex = CombineVertexData(vertexCombination[i], formatContainer).Span;
                combineVertex.CopyTo(resultVertexCombinationSpan[(i * 8)..]);
                vertexDataLength += combineVertex.Length;
            }
        }
        return resultVertexCombination[..vertexDataLength];
    }

    private static Vector3D<T> GetRelativeVertexVector(Vertex<T> vertex, VerticesType type) => type switch
    {
        VerticesType.Position => vertex.Position,
        VerticesType.Texture => new Vector3D<T>(vertex.TextureCoordinates, Vector3D<T>.Zero.Z),
        VerticesType.Color => vertex.ColorNormals,
        VerticesType.FacesInternal => vertex.ColorNormals,
        _ => Vector3D<T>.Zero
    };
}