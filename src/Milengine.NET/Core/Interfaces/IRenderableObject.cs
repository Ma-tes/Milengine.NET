using Silk.NET.Maths;

namespace Milengine.NET.Core.Interfaces;

public interface IRenderableObject
{
    public Vector3D<float> Position { get; set; }
    public Quaternion<float> Rotation { get; set; }
    public float Scale { get; set; }
    internal Matrix4X4<float> ViewMatrix { get; }

    public void OnInitialization();
    public void OnUpdate(float deltaTime);
    public void OnRender(float deltaTime);
}