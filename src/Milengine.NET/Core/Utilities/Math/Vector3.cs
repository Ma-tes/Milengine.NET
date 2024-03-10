namespace Milengine.NET.Core.Utilities.Math;

public struct Vector3 : IFactoryInstance<Vector3, float, float, float>
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    internal readonly Vector3 Zero => CreateInstance(0, 0, 0);

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3 CreateInstance(float argumentOne, float argumentTwo, float argumentThree) =>
        new Vector3(argumentOne, argumentTwo, argumentThree);
}

