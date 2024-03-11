using Silk.NET.Maths;

namespace Milengine.NET.Core.Interfaces;

public interface IRenderableObject
{
    public Vector3D<float> Position { get; set; }
    public Vector3D<float> Rotation { get; set; }

    public Task OnInitializationAsync();
    public Task OnUpdateAsync(float deltaTime);
}