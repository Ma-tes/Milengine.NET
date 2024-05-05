using Milengine.NET.Core;
using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.Material;
using Milengine.NET.Core.SceneManager;
using Milengine.NET.Core.Structures;
using Milengine.NET.Parser;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Milengine.NET_CreatingScene;

public sealed class SecondScene : SceneHolder
{
    private IKeyboard keyboard = null!;
    private IMouse mouse = null!;
    private Vector2D<float> lastMousePosition = Vector2D<float>.Zero;

    private float cameraVelocity = 25.0f;
    private bool isMouseEnable = false;

    public bool InputCurrentStatePress { get; private set; } = false;

    public SecondScene(IWindow window, Memory<ICamera> sceneCameras)
        : base(window, sceneCameras)
    {
    }

    public override void ExecuteObjectsInitialization()
    {
        GraphicsContext.Global.GraphicsInitialization();
        var inputManager = Window.CreateInput();
        keyboard = inputManager.Keyboards[0];
        mouse = inputManager.Mice[0];

        //TODO: Create more specific input handler, with
        //additional utilities for certain controlling.
        unsafe{
            GraphicsContext.Global.WindowGlfw.SetInputMode(GlfwWindowing.GetHandle(GraphicsContext.Global.Window),
                CursorStateAttribute.Cursor,
                CursorModeValue.CursorDisabled 
            );
        }
        GraphicsContext.Global.TextureMapper = new TextureMapper(
            @"/Users/mates/Downloads/TextingTextureMap.png",
            GLEnum.Texture2D,
            new Vector2D<int>(64, 64)
        );
        GraphicsContext.Global.TextureMapper.RenderParameters.Push(
            new TextureRenderParameter(GLEnum.TextureWrapS, (float)GLEnum.Repeat)
        );
        GraphicsContext.Global.TextureMapper.RenderParameters.Push(
            new TextureRenderParameter(GLEnum.TextureWrapT, (float)GLEnum.Repeat)
        );

        GraphicsContext.Global.TextureMapper.Bind();

        var objectModel = new ObjFormat();
        var currentTextures = GraphicsContext.Global.TextureMapper.Textures.Span;

        int texturesCount = currentTextures.Length;

        float planeAngle = 360.0f / texturesCount; 
        float planeRadius = 100.0f;
        for (int i = 0; i < texturesCount; i++)
        {
            float currentPlaneAngle = MathF.PI / 180.0f * (i * planeAngle);
            Vector3D<float> modelPosition = new Vector3D<float>(planeRadius * MathF.Cos(currentPlaneAngle), 0, planeRadius * MathF.Sin(currentPlaneAngle));
            RenderableObjects.Add(
                new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj"))
                {
                    TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[i],
                    Position = modelPosition
                });
        }
        base.ExecuteObjectsInitialization();
    }

    public override void ExecuteObjectsUpdate(double deltaTime)
    {
        base.ExecuteObjectsUpdate(deltaTime);
        if(CurrentCamera is ViewCamera viewCamera)
        {
            if(keyboard.IsKeyPressed(Key.W))
                viewCamera.Position +=
                    viewCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.S))
                viewCamera.Position -=
                    viewCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.A))
                viewCamera.Position -=
                    Vector3D.Normalize(Vector3D.Cross(
                        viewCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
                        viewCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value)) * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.D))
                viewCamera.Position +=
                    Vector3D.Normalize(Vector3D.Cross(
                        viewCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
                        viewCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value)) * cameraVelocity * (float)deltaTime;

            if(keyboard.IsKeyPressed(Key.Space))
                viewCamera.Position += Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.ShiftLeft))
                viewCamera.Position -= Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.ShiftLeft))
                viewCamera.Position -= Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;

            Vector2D<float> mousePosition = !isMouseEnable ? new Vector2D<float>(mouse.Position.X, mouse.Position.Y) : lastMousePosition;
            viewCamera.CalculateMouseViewDirections(mousePosition, lastMousePosition);
            lastMousePosition = mousePosition;
        }
        keyboard.KeyUp += (IKeyboard keyboard, Key key, int value) =>
        {
            if(key == Key.Escape)
            {
                unsafe{
                    WindowHandle* currentWindowHandle = GlfwWindowing.GetHandle(GraphicsContext.Global.Window);
                    CursorModeValue relativeCursorModeValue = isMouseEnable ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal;
                    GraphicsContext.Global.WindowGlfw.SetInputMode(currentWindowHandle,
                        CursorStateAttribute.Cursor,
                        relativeCursorModeValue
                    );
                }
                isMouseEnable = !isMouseEnable;
            }
            if(key == Key.E)
                SceneCameraMapper.MapIndex = (SceneCameraMapper.MapIndex + 1) % SceneCameraMapper.Values.Length;
        };
        Window.Title = Window.FramesPerSecond.ToString();
    }

    public override void ExecuteObjectsRender(double deltaTime)
    {
        GraphicsContext.Graphics.BindFramebuffer(GLEnum.Framebuffer, 0);
        base.ExecuteObjectsRender(deltaTime);
        GraphicsContext.Graphics.BindFramebuffer(GLEnum.DrawFramebuffer, 0);
        GraphicsContext.Graphics.BlitFramebuffer(
            0, 0, (int)GraphicsContext.Global.RelativeResolution.X, (int)GraphicsContext.Global.RelativeResolution.Y, 0, 0, Window.FramebufferSize.X, Window.FramebufferSize.Y,
            ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit,
            GLEnum.Nearest
        );
    }

    private float CalculateRelativeFieldOfView(float currentValue) =>
        Math.Clamp(currentValue, 0.1f,  179.9f);
}
