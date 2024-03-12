using Milengine.NET.Core.Graphics.Structures;

namespace Milengine.NET.Core.Graphics;

public class GraphicsMesh : IDisposable
{
    protected

    public Memory<Vertex<float>> Vertices { get; internal set; }
    public GraphicsBufferData<float> Indices { get; internal set; }


    public void Dispose() { }
}
