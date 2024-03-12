using Milengine.NET.Core.Graphics.Structures;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics;

public class GraphicsMesh : IDisposable
{
    
    protected virtual ReadOnlyMemory<FormatContainer> meshFormatContainer { get; } = new ReadOnlyMemory<FormatContainer>([
            new FormatContainer(index: 0, count: FormatCountType.Vector3D, type: VertexAttribPointerType.Float),
            new FormatContainer(index: 1, count: FormatCountType.Vector3D, type: VertexAttribPointerType.Float),
            new FormatContainer(index: 2, count: FormatCountType.Vector2D, type: VertexAttribPointerType.Float)]);

    internal MeshFormat MeshFormat { get; }
    internal VertexArrayBuffer<float, uint> VertexArrayBuffer { get; private set; }

    public GraphicsBufferData<float> Vertices { get; internal set; }
    public GraphicsBufferData<uint> Indices { get; internal set; }

    public GraphicsMesh(ReadOnlyMemory<Vertex<float>> vertices, ReadOnlyMemory<uint> indices)
    {
        Vertices = new GraphicsBufferData<float>(Vertex<float>.CreateMultipleVertexCombination(vertices.Span), BufferTargetARB.ArrayBuffer);
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

    public void Dispose()
    {
        VertexArrayBuffer.Dispose();
        Vertices.Dispose();
        Indices.Dispose();
    }
}
