using Milengine.NET.Core.Utilities;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Structures;

public enum Direction : uint
{
    Up = 0,
    Right = 2,
    Front = 4,
}

public sealed class DirectionValue
{
    public Direction Direction { get; }
    public Vector3D<float> Value { get; set; }

    public DirectionValue(Direction direction, Vector3D<float> value)
    {
        Direction = direction;
        Value = value;
    }
}

public struct CameraConfiguration 
{
    public float FieldOfView { get; }
    public float Zoom { get; }
    public float Sensivity { get; }

    public float ClippingPlaneNear { get; set; }
    public float ClippingPlaneFar { get; set; }

    public Memory<DirectionValue> CameraDirections { get; set; }

    public CameraConfiguration(float fieldOfView, float zoom,
        float sensivity, float clippingPlaneNear, float clippingPlaneFar, InlineParameter_Three<DirectionValue> directions)
    {
        FieldOfView = fieldOfView;
        Zoom = zoom;
        Sensivity = sensivity;
        ClippingPlaneNear = clippingPlaneNear;
        ClippingPlaneFar = clippingPlaneFar;
        CameraDirections = SpanHelper<DirectionValue>.CreateFixedParameterSpan(ref directions).ToArray();
    }

    public readonly ref DirectionValue GetRelativeDirectionValue(Direction direction)
    {
        int directionValues = InlineParameter_Three.Length;
        Span<DirectionValue> directionValuesSpan = CameraDirections.Span;
        for (int i = 0; i < directionValues; i++)
        {
            ref DirectionValue currentDirectionValue = ref directionValuesSpan[i];
            if(currentDirectionValue.Direction == direction)
                return ref currentDirectionValue;
        }
        throw new ArgumentException($"Current camera directions do not contain specific direction: {direction}");
    }
}