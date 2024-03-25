using System.Numerics;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Interfaces;

public interface IRenderableObject
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public float Scale { get; set; }
    internal Matrix4x4 ViewMatrix { get; }

    public void OnInitialization();
    public void OnUpdate(float deltaTime);
    public void OnRender(float deltaTime);
}