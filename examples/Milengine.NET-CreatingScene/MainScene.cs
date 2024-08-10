using System.Reflection;
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

public sealed class MainScene : SceneHolder
{
    internal readonly static string EnvironmentAssetsPath = Path.Combine(Environment.CurrentDirectory, "assets/");

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
        string currentPath = EnvironmentAssetsPath;
        GraphicsContext.Global.TextureMapper = new TextureMapper(
            EnvironmentAssetsPath + "TextureMap.png",
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
                new Model(objectModel.LoadFormatModelData(EnvironmentAssetsPath + "Floor.obj"))
                {
                    TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[i],
                    Position = modelPosition
                });
        }
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(EnvironmentAssetsPath + "CharacterOne.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[0]
            });
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(EnvironmentAssetsPath + "Rifle.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[2],
                Position = Vector3D<float>.UnitX * 20.0f,
                Scale = 0.02f
            });
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(EnvironmentAssetsPath + "Weapon.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[2],
                Position = Vector3D<float>.UnitX * 40.0f,
                Scale = 0.2f
            });
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(EnvironmentAssetsPath + "CharacterTwo.obj"))
            {
                TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[1],
                Position = Vector3D<float>.UnitX * 100.0f
            });

        float planetMaxPosition = 100.0f;
        int planetCount = 10;
        for(int i = 0; i < planetCount; i++)
        {
            var planetPosition = new Vector3D<float>(
                (float)Random.Shared.NextDouble() * planetMaxPosition * MathF.Sin((float)Window.Time * 5),
                (float)Random.Shared.NextDouble() * planetMaxPosition,
                (float)Random.Shared.NextDouble() * planetMaxPosition  * MathF.Cos((float)Window.Time * 5)
            );

            int currentTextureIndex = i % texturesCount;
            RenderableObjects.Add(
                new Model(objectModel.LoadFormatModelData(EnvironmentAssetsPath + "Planet.obj"))
                {
                    TextureTemporaryHolder = GraphicsContext.Global.TextureMapper.Textures.Span[currentTextureIndex],
                    Position = planetPosition,
                    Scale = 2.0f
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
        RenderableObjects[9].Rotation = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitX, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitZ, 1.0f * (float)Window.Time);
        RenderableObjects[10].Rotation = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitX, 1.0f * (float)Window.Time)
            * Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitZ, 1.0f * (float)Window.Time);
        
        RenderableObjects[10].Position += Vector3D<float>.UnitX * (MathF.Sin((float)Window.Time) / 100);
         
        RenderableObjects[11].Position += Vector3D<float>.UnitY * MathF.Sin((float)Window.Time);
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
