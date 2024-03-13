using Silk.NET.Maths;

namespace Milengine.NET.Core.Interfaces;

public interface IAsyncRenderableObject
{
    public Vector3D<float> Position { get; set; }
    public Vector3D<float> Rotation { get; set; }

    public ValueTask OnInitializationAsync();
    public ValueTask OnUpdateAsync(float deltaTime);
}

public interface IRenderableObject
{
    public Vector3D<float> Position { get; set; }
    public Vector3D<float> Rotation { get; set; }

    public void OnInitialization();
    public void OnUpdate(float deltaTime);
}