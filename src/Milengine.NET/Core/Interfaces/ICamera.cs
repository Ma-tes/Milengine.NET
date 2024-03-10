using Milengine.NET.Core.Structures;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Interfaces;

public interface ICamera
{
    public Vector3D<float> Position { get; set; }
    public CameraConfiguration CameraConfiguration { get; set; }
}