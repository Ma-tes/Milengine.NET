using Milengine.NET.Core.Graphics.Structures;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics;

public class GraphicsMesh : IDisposable
{
    protected virtual ReadOnlyMemory<FormatContainer> meshFormatContainer { get; set; } = new ReadOnlyMemory<FormatContainer>([
            new FormatContainer(verticesType: VerticesType.Position, count: FormatCountType.Vector3D, type: VertexAttribPointerType.Float),
            new FormatContainer(verticesType: VerticesType.Color, count: FormatCountType.Vector3D, type: VertexAttribPointerType.Float),
            new FormatContainer(verticesType: VerticesType.Texture, count: FormatCountType.Vector2D, type: VertexAttribPointerType.Float)]);

    internal MeshFormat MeshFormat { get; }
    internal VertexArrayBuffer<float, uint> VertexArrayBuffer { get; private set; }

    public GraphicsBufferData<float> Vertices { get; internal set; }
    public GraphicsBufferData<uint> Indices { get; internal set; }

    public GraphicsMesh(ReadOnlyMemory<Vertex<float>> vertices, ReadOnlyMemory<uint> indices)
    {
        Vertices = new GraphicsBufferData<float>(Vertex<float>.CreateMultipleVertexCombination(vertices.Span, meshFormatContainer), BufferTargetARB.ArrayBuffer);
        Indices = new GraphicsBufferData<uint>(indices, BufferTargetARB.ElementArrayBuffer);
        MeshFormat = new MeshFormat(meshFormatContainer);
    }

    //TODO: Use for return a specific result type, for
    //better exception handling.
    public virtual void LoadMesh()
    {
        VertexArrayBuffer = new VertexArrayBuffer<float, uint>(Vertices, Indices);
        VertexArrayBuffer.Bind();
        MeshFormat.Bind();
    }
    //TODO: Implement the specific mesh updating.

    public void Dispose()
    {
        VertexArrayBuffer.Dispose();
        Vertices.Dispose();
        Indices.Dispose();
    }
}