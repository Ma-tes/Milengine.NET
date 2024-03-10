using Milengine.NET.Core.Utilities.Math;

namespace Milengine.NET.Core.Structures;

public enum Direction : uint
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
}

public sealed record DirectionValue(Direction Direction, Vector3 Value);

public readonly struct CameraConfiguration 
{
    public float FieldOfView { get; }
    public float Zoom { get; }
    public float Sensivity { get; }
    public ReadOnlyMemory<DirectionValue> CameraDirections { get; }

    public CameraConfiguration(float fieldOfView, float zoom,
        float sensivity, params DirectionValue[] directionValues)
    {
        FieldOfView = fieldOfView;
        Zoom = zoom;
        Sensivity = sensivity;
    }

    public CameraConfiguration(float fieldOfView, float zoom,
        float sensivity, Span<Direction> directionValues)
    {
        FieldOfView = fieldOfView;
        Zoom = zoom;
        Sensivity = sensivity;
    }

    //private ReadOnlyMemory<DirectionValue> GetRelativeCameraDirections(Span<Direction> directions)
}

