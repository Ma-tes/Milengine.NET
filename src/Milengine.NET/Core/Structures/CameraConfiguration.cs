using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Structures;

public enum Direction : uint
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
}

public sealed record DirectionValue(Direction Direction, Vector3D<float> Value);

public readonly struct CameraConfiguration 
{
    public float FieldOfView { get; }
    public float Zoom { get; }
    public float Sensivity { get; }

    public InlineValueParameter_Four<DirectionValue> CameraDirections { get; }


    public CameraConfiguration(float fieldOfView, float zoom,
        float sensivity, InlineValueParameter_Four<DirectionValue> directions)
    {
        FieldOfView = fieldOfView;
        Zoom = zoom;
        Sensivity = sensivity;
        CameraDirections = directions;
    }
}