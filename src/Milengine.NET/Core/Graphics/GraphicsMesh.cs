using Milengine.NET.Core.Graphics.Enums;
using Milengine.NET.Core.Graphics.Structures;
using Milengine.NET.Core.Utilities;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Graphics;

public class GraphicsMesh : IFactoryInstance<GraphicsMesh, ReadOnlyMemory<Vertex<float>>, ReadOnlyMemory<uint>>, IDisposable
{
    protected virtual ReadOnlyMemory<FormatContainer> meshFormatContainer { get; set; } = new ReadOnlyMemory<FormatContainer>([
            new FormatContainer(verticesType: VerticesType.Position, count: FormatCountType.Vector3D, type: VertexAttribPointerType.Float),
            new FormatContainer(verticesType: VerticesType.Color, count: FormatCountType.Vector3D, type: VertexAttribPointerType.Float),
            new FormatContainer(verticesType: VerticesType.Texture, count: FormatCountType.Vector2D, type: VertexAttribPointerType.Float)]);

    internal MeshFormat MeshFormat { get; }
    internal VertexArrayBuffer<float, uint> VertexArrayBuffer { get; private set; }
 
    public GraphicsBufferData<float> Vertices { get; internal set; }
    public GraphicsBufferData<uint> Indices { get; internal set; }

    public static GraphicsMesh CreateInstance(ReadOnlyMemory<Vertex<float>> argumentOne,
     ReadOnlyMemory<uint> argumentTwo) => new(argumentOne, argumentTwo);

    public static GraphicsMesh CreateInstance(Vertex<float>[] argumentOne,
     uint[] argumentTwo) => new(argumentOne, argumentTwo);

    public GraphicsMesh(Vertex<float>[] vertices, uint[] indices)
    {
        Vertices = new GraphicsBufferData<float>(Vertex<float>.CreateMultipleVertexCombination(vertices, meshFormatContainer), BufferTargetARB.ArrayBuffer);
        Indices = new GraphicsBufferData<uint>(indices, BufferTargetARB.ElementArrayBuffer);
        MeshFormat = new MeshFormat(meshFormatContainer);
    }

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