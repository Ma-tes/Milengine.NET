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
    private bool isMouseHold = false;

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
        Vector2D<float> mousePosition = !isMouseEnable ? new Vector2D<float>(mouse.Position.X, mouse.Position.Y) : lastMousePosition;
        base.ExecuteObjectsUpdate(deltaTime);
        if(keyboard.IsKeyPressed(Key.W))
            CurrentCamera.Position +=
                CurrentCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value * cameraVelocity * (float)deltaTime;
        if(keyboard.IsKeyPressed(Key.S))
            CurrentCamera.Position -=
                CurrentCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value * cameraVelocity * (float)deltaTime;
        if(keyboard.IsKeyPressed(Key.A))
            CurrentCamera.Position -=
                Vector3D.Normalize(Vector3D.Cross(
                    CurrentCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
                    CurrentCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value)) * cameraVelocity * (float)deltaTime;
        if(keyboard.IsKeyPressed(Key.D))
            CurrentCamera.Position +=
                Vector3D.Normalize(Vector3D.Cross(
                    CurrentCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Front).Value,
                    CurrentCamera.CameraConfiguration.GetRelativeDirectionValue(Direction.Up).Value)) * cameraVelocity * (float)deltaTime;


        if(CurrentCamera is ViewCamera viewCamera)
        {
            if(keyboard.IsKeyPressed(Key.Space))
                CurrentCamera.Position += Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.ShiftLeft))
                CurrentCamera.Position -= Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;

            viewCamera.CalculateMouseViewDirections(mousePosition, lastMousePosition);
            lastMousePosition = mousePosition;
        }
        if(CurrentCamera is SceneCamera sceneCamera)
        {
            if(keyboard.IsKeyPressed(Key.N))
                CurrentCamera.CameraConfiguration.Zoom = Math.Clamp(CurrentCamera.CameraConfiguration.Zoom + 0.1f, 1.0f, 100.0f);
            if(keyboard.IsKeyPressed(Key.M))
                CurrentCamera.CameraConfiguration.Zoom = Math.Clamp(CurrentCamera.CameraConfiguration.Zoom - 0.1f, 1.0f, 100.0f);

            isMouseEnable = false;
            mouse.MouseDown += (IMouse mouse, Silk.NET.Input.MouseButton button) => { lastMousePosition = new(mouse.Position.X, mouse.Position.Y); isMouseHold = true; };
            mouse.MouseUp += (IMouse mouse, Silk.NET.Input.MouseButton button) => { isMouseHold = false; };

            if(isMouseHold)
            {
                float relativePositionX = mouse.Position.X - lastMousePosition.X;
                float relativePositionY = mouse.Position.Y - lastMousePosition.Y;
                sceneCamera.Position -= Vector3D<float>.UnitX * relativePositionX * (cameraVelocity / 10) * (float)deltaTime;
                sceneCamera.Position += Vector3D<float>.UnitY * relativePositionY * (cameraVelocity / 10) * (float)deltaTime;
            }
            lastMousePosition = new(mouse.Position.X, mouse.Position.Y);
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
