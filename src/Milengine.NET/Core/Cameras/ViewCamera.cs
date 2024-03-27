using System.Collections.Immutable;
using System.Numerics;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Structures;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.Structures;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Cameras;

public sealed class ViewCamera : ICamera, IRenderableObject
{
    private static readonly ImmutableArray<Vertex<float>> graphicsMeshVertexData = [
        new Vertex<float>(
            new(0.25f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.75f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.25f, 0.75f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.75f, 0.75f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),

        new Vertex<float>(
            new(0.0f, 0.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(1.0f, 0.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),

        new Vertex<float>(
            new(0.25f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.0f, 0.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.75f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(1.0f, 0.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.25f, 0.75f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.75f, 0.75f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),

        new Vertex<float>(
            new(0.25f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.25f, 0.75f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.75f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.75f, 0.75f, 0.0f),
            new(1.0f, 0.0f, 0.0f)
        ),

        new Vertex<float>(
            new(0.0f, 0.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(0.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(1.0f, 0.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
        new Vertex<float>(
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 0.0f)
        ),
    ];

    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
    public Quaternion<float> Rotation { get; set; } = Quaternion<float>.Identity;

    public float Scale { get; set; } = 5.0f;
    public Matrix4X4<float> ViewMatrix =>
        Matrix4X4<float>.Identity * Matrix4X4.CreateFromQuaternion(Rotation)
        * Matrix4X4.CreateScale(Scale) * Matrix4X4.CreateTranslation(Position);

    public float Yaw { get; internal set; } = -90.0f;
    public float Pitch { get; internal set; } = 0.0f;

    public GraphicsMesh CameraViewModel { get; set; }
    public bool IsRenderable { get; } = true;

    public CameraConfiguration CameraConfiguration { get; set; } = new(
        fieldOfView: 60.0f,
        zoom: 1.0f,
        //Pixels per second
        sensivity: 1.0f,
        InlineValueParameter_Three<DirectionValue>.CreateInstance(
            new DirectionValue(Direction.Up, new(0.0f, 1.0f, 0.0f)),
            new DirectionValue(Direction.Right, new(1.0f, 0.0f, 0.0f)),
            new DirectionValue(Direction.Front, new(0.0f, 0.0f, -1.0f))
        )
    );

    public ViewCamera() { }

    public void OnInitialization() 
    {
        CameraViewModel = new GraphicsMesh([..graphicsMeshVertexData], []);
        CameraViewModel.LoadMesh();
    }
    public void OnUpdate(float deltaTime) { }

    public void OnRender(float deltaTime)
    {
        unsafe
        {
            Matrix4X4<float> currentModelMatrix = ViewMatrix;
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uModel"),
                1, false, (float*)&currentModelMatrix);
        }
        CameraViewModel.VertexArrayBuffer.Bind(); 
        unsafe { GraphicsContext.Graphics.DrawArrays(GLEnum.Lines, 0, (uint)CameraViewModel.Vertices.Buffer.Length); }
    }

    public Matrix4X4<float> CalculateCameraView()
    {
        var currentView = Matrix4X4.CreateLookAt(
            Position,
            Position + CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
            CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value 
        );
        return currentView;
    }


    public void CalculateMouseViewDirections(Vector2D<float> mousePosition, Vector2D<float> lastMousePosition)
    {
        float relativeXPosition = mousePosition.X - lastMousePosition.X;
        float relativeYPosition = lastMousePosition.Y - mousePosition.Y;

        Yaw += relativeXPosition; 
        Pitch = Math.Clamp(Pitch + relativeYPosition, -89.0f, 89.0f);
        Console.WriteLine($"X: {relativeXPosition}\nY: {relativeYPosition}\n");
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
        Rotation = new Quaternion<float>(
            CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value
                * 1.0f, 0
        );
    }

    public Matrix4X4<float> CalculateProjectionView() =>
        Matrix4X4.CreatePerspectiveFieldOfView(
            GetRadiansFromDegrees(CameraConfiguration.FieldOfView), GraphicsContext.Global.Window.FramebufferSize.X / GraphicsContext.Global.Window.FramebufferSize.Y, 25f, 100.0f
        );

    private static Vector2D<float> CalculateRelativeMouseDirection(Vector2 mousePosition)
    {
        float frameBufferX = GraphicsContext.Global.Window.FramebufferSize.X / 2;
        float frameBufferY = GraphicsContext.Global.Window.FramebufferSize.Y / 2; 
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
        MathF.PI / 180.0f * degree;
}