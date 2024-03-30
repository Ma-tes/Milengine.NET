using Milengine.NET.Core;
using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.SceneManager;
using Milengine.NET.Core.Structures;
using Milengine.NET.Parser;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Milengine.NET.Examples.CoreTest;

public sealed class MainScene : SceneHolder
{
    private IKeyboard keyboard = null!;
    private IMouse mouse = null!;
    private Vector2D<float> lastMousePosition = Vector2D<float>.Zero;

    private float cameraVelocity = 25.0f;
    private bool isMouseEnable = false;

    public bool InputCurrentStatePress { get; private set; } = false;

    public MainScene(IWindow window, Memory<ICamera> sceneCameras)
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

        var objectModel = new ObjFormat();
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj"))
            {
                TextureTemporaryHolder = new Core.Texture("/Users/mates/Downloads/RETRO_TEXTURE_PACK_SAMPLE/SAMPLE/BRICK_1A.png", GLEnum.Texture2D)
            });
        //RenderableObjects.Add(
        //    new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Char1.obj")));
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
                MainCameraIndex = (MainCameraIndex + 1) % SceneCameras.Length;
        };
        //RenderableObjects[3].Rotation = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, 0.5f * (float)Window.Time);
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
}
