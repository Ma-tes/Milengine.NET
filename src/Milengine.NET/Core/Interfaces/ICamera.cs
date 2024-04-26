using Milengine.NET.Core.Camera.Structures;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Interfaces;

public interface ICamera
{
    public CameraConfiguration CameraConfiguration { get; set; }
    public bool IsRenderable { get; }

    public Matrix4X4<float> CalculateCameraView();
    public Matrix4X4<float> CalculateProjectionView();
}