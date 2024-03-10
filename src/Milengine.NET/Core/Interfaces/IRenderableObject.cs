using Milengine.NET.Core.Utilities.Math;

namespace Milengine.NET.Core.Interfaces;

public interface IRenderableObject
{
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }

    public Task OnInitializationAsync();
    public Task OnUpdateAsync(float deltaTime);
}