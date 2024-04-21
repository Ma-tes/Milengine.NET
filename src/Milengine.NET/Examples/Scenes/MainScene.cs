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

namespace Milengine.NET.Examples;

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
        for (int i = 0; i < texturesCount; i++)
        {
            Vector3D<float> modelPosition = new Vector3D<float>(20 * i, 0, 0);
            RenderableObjects.Add(
                new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj"))
                {
                    TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[i],
                    Position = modelPosition
                });
        }
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Char1.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[0]
            });
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/ass_rifle.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[2],
                Position = Vector3D<float>.UnitX * 20.0f,
                Scale = 0.02f
            });
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/pistol.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[2],
                Position = Vector3D<float>.UnitX * 40.0f,
                Scale = 0.2f
            });
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/mecha.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[1],
                Position = Vector3D<float>.UnitX * 100.0f
            });

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

            if(keyboard.IsKeyPressed(Key.O))
                viewCamera.CameraConfiguration.FieldOfView = CalculateRelativeFieldOfView(viewCamera.CameraConfiguration.FieldOfView + 0.25f);
            if(keyboard.IsKeyPressed(Key.P))
                viewCamera.CameraConfiguration.FieldOfView = CalculateRelativeFieldOfView(viewCamera.CameraConfiguration.FieldOfView - 0.25f);

            if(keyboard.IsKeyPressed(Key.Number1))
                GraphicsContext.Global.CurrentRenderingType = GLEnum.Triangles;
            if(keyboard.IsKeyPressed(Key.Number2))
                GraphicsContext.Global.CurrentRenderingType = GLEnum.LineStrip; 

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

        int objectsCount = RenderableObjects.Count;
        RenderableObjects[objectsCount - 3].Rotation = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitX, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitZ, 1.0f * (float)Window.Time);
        RenderableObjects[objectsCount - 3].Position += MathF.Sin((float)Window.Time * 5) * (Vector3D<float>.UnitY * 0.5f);

        RenderableObjects[objectsCount - 2].Rotation = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitX, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitZ, 1.0f * (float)Window.Time);
        RenderableObjects[objectsCount - 2].Position += MathF.Sin((float)Window.Time * 5) * (Vector3D<float>.UnitY * 0.5f);
        Window.Title = Window.FramesPerSecond.ToString();
        Console.WriteLine(CurrentFrameTick.CalculateRelativeFramesPerSecond());
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
