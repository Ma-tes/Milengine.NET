using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.Camera.Structures;
using Silk.NET.Maths;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;

namespace Milengine.NET.Core.Cameras;

public sealed class SceneCamera : ICamera
{
    public Vector3D<float> Position { get; set; } = Vector3D<float>.UnitZ * -1;

    public CameraConfiguration CameraConfiguration { get; set; } = new(
        fieldOfView: 90.0f,
        zoom: 10.0f,
        sensivity: 1.0f,
        clippingPlaneNear: 0.1f,
        clippingPlaneFar: 500f,
        InlineValueParameter_Three<DirectionValue>.CreateInstance(
            new DirectionValue(Direction.Up, new(0.0f, 1.0f, 0.0f)),
            new DirectionValue(Direction.Right, new(1.0f, 0.0f, 0.0f)),
            new DirectionValue(Direction.Front, new(0.0f, 0.0f, -1.0f))
        )
    );

    public bool IsRenderable { get; } = false;

    public Matrix4X4<float> CalculateCameraView() =>
        Matrix4X4.CreateLookAt(
            Position,
            Position + CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
            Vector3D<float>.UnitY
        ) * Matrix4X4.CreateRotationX(GetRadiansFromDegrees(90));
 
    public Matrix4X4<float> CalculateProjectionView()
    {
        Vector2D<int> bufferSize = GraphicsContext.Global.Window.FramebufferSize;
        var relativeHorizontalPosition = new Vector2D<float>(
            Position.X - (bufferSize.X / 2),
            Position.X + (bufferSize.X / 2)
        );
        var relativeVerticalPosition = new Vector2D<float>(
            Position.Y - (bufferSize.Y / 2),
            Position.Y + (bufferSize.Y / 2)
        );

        return Matrix4X4.CreateOrthographicOffCenter(relativeHorizontalPosition.X, relativeVerticalPosition.Y,
            relativeVerticalPosition.X, relativeVerticalPosition.Y,
            CameraConfiguration.ClippingPlaneNear, CameraConfiguration.ClippingPlaneFar)
                * Matrix4X4.CreateScale(CameraConfiguration.Zoom);
    }

    private static float GetRadiansFromDegrees(float degree) =>
        MathF.PI / 180.0f * degree;
}
