using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Silk.NET.Maths;

namespace Milengine.NET.Core;

public class Model : IRenderableObject
{
    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
    public Vector3D<float> Rotation { get; set; } = Vector3D<float>.Zero;

    public ReadOnlyMemory<GraphicsMesh> Meshes { get; protected set; }

    public Model(ReadOnlyMemory<GraphicsMesh> meshes)
    {
        Meshes = meshes;
    }

    public virtual void OnInitialization()
    {
        int meshesLength = Meshes.Length;
        var meshesSpan = Meshes.Span;
        for (int i = 0; i < meshesLength; i++) { meshesSpan[i].LoadMesh(); }
    }

    //TODO: Implement the specific model updating of related meshes.
    public virtual void OnUpdate(float deltaTime) { }
}
