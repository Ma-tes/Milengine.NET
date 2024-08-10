using Milengine.NET.Core.Camera.Structures;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Structures;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Cameras;

public sealed class ViewCamera : ICamera, IRenderableObject
{
    private Memory<Vertex<float>> graphicsMeshVertexData;
 
    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
    public Quaternion<float> Rotation { get; set; } = Quaternion<float>.Identity;

    public float Scale { get; set; } = 5.0f;
    public Matrix4X4<float> ViewMatrix =>
        Matrix4X4<float>.Identity * Matrix4X4.CreateRotationX(GetRadiansFromDegrees(-Pitch)) * Matrix4X4.CreateRotationY(GetRadiansFromDegrees(90 - Yaw))
            * Matrix4X4.CreateScale(Scale) * Matrix4X4.CreateTranslation(new Vector3D<float>(Position.X, Position.Y, Position.Z));

    public float Yaw { get; internal set; } = -90.0f;
    public float Pitch { get; internal set; } = 0.0f;

    public GraphicsMesh CameraViewModel { get; set; }
    public bool IsRenderable { get; } = true;

    public CameraConfiguration CameraConfiguration { get; set; } = new(
        fieldOfView: 90.0f,
        zoom: 1.0f,
        sensivity: 1.0f,
        clippingPlaneNear: 0.1f,
        clippingPlaneFar: 500f,
        InlineValueParameter_Three<DirectionValue>.CreateInstance(
            new DirectionValue(Direction.Up, new(0.0f, 1.0f, 0.0f)),
            new DirectionValue(Direction.Right, new(1.0f, 0.0f, 0.0f)),
            new DirectionValue(Direction.Front, new(0.0f, 0.0f, -1.0f))
        )
    );

    public ViewCamera() { }

    public void OnInitialization()
    {
        graphicsMeshVertexData = CreateCameraMeshVertices(CameraConfiguration.ClippingPlaneNear, CameraConfiguration.ClippingPlaneFar);
        CameraViewModel = new GraphicsMesh([..graphicsMeshVertexData.Span], []);
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
            GetRadiansFromDegrees(CameraConfiguration.FieldOfView),
                GraphicsContext.Global.Window.FramebufferSize.X / GraphicsContext.Global.Window.FramebufferSize.Y,
                    CameraConfiguration.ClippingPlaneNear, CameraConfiguration.ClippingPlaneFar
    );

    private static Memory<Vertex<float>> CreateCameraMeshVertices(float nearPlane, float farPlane)
    {
        Memory<Vertex<float>> vertices = new Vertex<float>[32];
        Span<Vertex<float>> verticesSpan = vertices.Span;
        InlineParameter_Two<float> planes = InlineValueParameter_Two<float>.CreateInstance(
            nearPlane, farPlane
        );

        int planesLength = InlineParameter_Two<float>.Length;
        for (int i = 0; i < planesLength; i++)
        {
            float verticesPosition = Math.Clamp(planes[i] / 50, 1.0f, 500.0f);
            float verticesYPositionIndexer = i * -0.5f;

            int relativeVerticesIndex = i * 16;
            verticesSpan[relativeVerticesIndex] = new(new Vector3D<float>(-1.0f * verticesPosition,
                verticesYPositionIndexer + 0.0f, verticesPosition));
            verticesSpan[relativeVerticesIndex + 1] = new(new Vector3D<float>(1.0f * verticesPosition,
                verticesYPositionIndexer + 0.0f, verticesPosition));
            verticesSpan[relativeVerticesIndex + 2] = new(new Vector3D<float>(1.0f * verticesPosition,
                verticesYPositionIndexer + 1.0f * verticesPosition, verticesPosition));
            verticesSpan[relativeVerticesIndex + 3] = new(new Vector3D<float>(-1.0f * verticesPosition,
                verticesYPositionIndexer + 1.0f * verticesPosition, verticesPosition));
            verticesSpan[relativeVerticesIndex + 4] = new(new Vector3D<float>(-1.0f * verticesPosition,
                verticesYPositionIndexer + 0.0f, verticesPosition));
            verticesSpan[relativeVerticesIndex + 5] = new(new Vector3D<float>(-1.0f * verticesPosition,
                verticesYPositionIndexer + 1.0f * verticesPosition, verticesPosition));
            verticesSpan[relativeVerticesIndex + 6] = new(new Vector3D<float>(1.0f * verticesPosition,
                verticesYPositionIndexer + 0.0f, verticesPosition));
            verticesSpan[relativeVerticesIndex + 7] = new(new Vector3D<float>(1.0f * verticesPosition,
                verticesYPositionIndexer + 1.0f * verticesPosition, verticesPosition));

            int relativePlaneLength = 4;
            for (int j = 0; j < relativePlaneLength; j++)
            {
                int currentIndex = relativeVerticesIndex + 8 + (j * 2);
                verticesSpan[currentIndex] = new(new Vector3D<float>(0.0f, 0.5f, 0.0f));
                verticesSpan[currentIndex + 1] = verticesSpan[relativeVerticesIndex + j];
            }
        }
        return vertices;
    }

    private static float GetRadiansFromDegrees(float degree) =>
        MathF.PI / 180.0f * degree;
}