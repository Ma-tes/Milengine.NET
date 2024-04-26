using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Utilities;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;

namespace Milengine.NET.Core.Camera.Structures;

public sealed class CameraConfiguration
{
    public float FieldOfView { get; set; }
    public float Zoom { get; set; }
    public float Sensivity { get; set; }

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

    public ref DirectionValue GetRelativeDirectionValue(Direction direction)
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