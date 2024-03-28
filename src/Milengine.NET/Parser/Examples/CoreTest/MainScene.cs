using System.Numerics;
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
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Milengine.NET.Examples.CoreTest;

public sealed class MainScene : SceneHolder
{
    private IKeyboard keyboard = null!;
    private IMouse mouse = null!;
    private Vector2D<float> lastMousePosition = Vector2D<float>.Zero;

    private float cameraVelocity = 25.0f;

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
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj")));
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Char1.obj")));
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Medkit.obj")){
                 Position = new(0.0f, 0.0f, 10.0f)});

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

            Vector2D<float> mousePosition = new Vector2D<float>(mouse.Position.X, mouse.Position.Y);
            viewCamera.CalculateMouseViewDirections(mousePosition, lastMousePosition);
            lastMousePosition = mousePosition;
        }
        if(keyboard.IsKeyPressed(Key.E))
        {
            int cameraCount = SceneCameras.Span.Length;
            MainCameraIndex = (MainCameraIndex + 1) % cameraCount;
        }
        RenderableObjects[3].Rotation = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, 0.5f * (float)Window.Time);
    }

    public override void ExecuteObjectsRender(double deltaTime)
    {
        base.ExecuteObjectsRender(deltaTime);
    }
}
