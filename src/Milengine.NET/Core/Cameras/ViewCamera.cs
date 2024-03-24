using System.Numerics;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.Structures;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;

namespace Milengine.NET.Core.Cameras;

public sealed class ViewCamera : ICamera
{
    public Vector3D<float> Position { get; set; }

    public float Yaw { get; internal set; } = -90.0f;
    public float Pitch { get; internal set; } = 0.0f;

    public CameraConfiguration CameraConfiguration { get; set; } = new(
        fieldOfView: 60.0f,
        zoom: 1.0f,
        //Pixels per second
        sensivity: 1.0f,
        InlineValueParameter_Three<DirectionValue>.CreateInstance(
            new DirectionValue(Direction.Up, new(0.0f, 1.0f, 0.0f)),
            new DirectionValue(Direction.Right, new(1.0f, 0.0f, 0.0f)),
            new DirectionValue(Direction.Front, new(0.0f, 0.0f, 1.0f))
        )
    );

    public ViewCamera() { }

    public Matrix4X4<float> CalculateCameraView() =>
        Matrix4X4.CreateLookAt(
            Position,
            Position + CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
            CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value 
        );

    public void CalculateMouseViewDirections(Vector2 mousePosition)
    {
        var relativeMousePosition = CalculateRelativeMouseDirection(mousePosition) * CameraConfiguration.Sensivity;
        Yaw += relativeMousePosition.X; 
        Pitch = Math.Clamp(Pitch + relativeMousePosition.Y, -89.0f, 89.0f);
        Console.WriteLine($"X: {relativeMousePosition.X}\nY: {relativeMousePosition.Y}\n");
        Console.WriteLine($"Pitch: {Pitch}");

        CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value = Vector3D.Normalize(new Vector3D<float>(
            MathF.Cos(GetRadiansFromDegrees(Yaw)) * MathF.Cos(GetRadiansFromDegrees(Pitch)),
            MathF.Sin(GetRadiansFromDegrees(Pitch)),
            MathF.Sin(GetRadiansFromDegrees(Yaw)) * MathF.Cos(GetRadiansFromDegrees(Pitch))
        ));

        CameraConfiguration.GetRelativeDirectionValue(Direction.Right).Value = Vector3D.Normalize(
            Vector3D.Cross(CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
                Vector3D<float>.UnitY)
        );
        CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value = Vector3D.Normalize(
            Vector3D.Cross(CameraConfiguration.GetRelativeDirectionValue(Direction.Right).Value,
                CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value)
        );
    }

    public Matrix4X4<float> CalculateProjectionView() =>
        Matrix4X4.CreatePerspectiveFieldOfView(
            GetRadiansFromDegrees(CameraConfiguration.FieldOfView), GraphicsContext.Global.Window.FramebufferSize.X / GraphicsContext.Global.Window.FramebufferSize.Y, 25f, 100.0f
        );

    private static Vector2D<float> CalculateRelativeMouseDirection(Vector2 mousePosition)
    {
        float frameBufferX = GraphicsContext.Global.Window.FramebufferSize.X;
        float frameBufferY = GraphicsContext.Global.Window.FramebufferSize.Y; 
        if(mousePosition.X < 0 || mousePosition.X > frameBufferX ||
            mousePosition.Y < 0 || mousePosition.Y > frameBufferY)
            return Vector2D<float>.Zero;

        float centerX = frameBufferX / 2;
        float centerY = frameBufferY / 2;
        return new Vector2D<float>(
            centerX - mousePosition.X,
            centerY - mousePosition.Y
        );
    }

    private static float GetRadiansFromDegrees(float degree) =>
        MathF.PI * degree / 180.0f;
}
