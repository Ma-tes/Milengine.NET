using Silk.NET.Maths;

namespace Milengine.NET.Core.Cameras;

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
