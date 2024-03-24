using System.Numerics;
using Milengine.NET.Core;
using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.SceneManager;
using Milengine.NET.Parser;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Milengine.NET.Examples.CoreTest;

public sealed class MainScene : SceneHolder
{
    private IKeyboard keyboard = null!;
    private IMouse mouse = null!;

    private float cameraVelocity = 25.0f;

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

        ObjFormat objectModel = new ObjFormat();
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj")));
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Char1.obj")));
        RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Medkit.obj")));
        RenderableObjects[0].Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 120);
        
        base.ExecuteObjectsInitialization();
    }

    public override void ExecuteObjectsUpdate(double deltaTime)
    {
        base.ExecuteObjectsUpdate(deltaTime);
        if(CurrentCamera is ViewCamera viewCamera)
        {
            if(keyboard.IsKeyPressed(Key.W))
                viewCamera.Position -= Vector3D<float>.UnitZ * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.S))
                viewCamera.Position += Vector3D<float>.UnitZ * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.A))
                viewCamera.Position -= Vector3D<float>.UnitX * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.D))
                viewCamera.Position += Vector3D<float>.UnitX * cameraVelocity * (float)deltaTime;

            if(keyboard.IsKeyPressed(Key.Space))
                viewCamera.Position += Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;
            if(keyboard.IsKeyPressed(Key.ShiftLeft))
                viewCamera.Position -= Vector3D<float>.UnitY * cameraVelocity * (float)deltaTime;

            viewCamera.CalculateMouseViewDirections(mouse.Position);
        }
    }

    public override void ExecuteObjectsRender(double deltaTime)
    {
        base.ExecuteObjectsRender(deltaTime);
    }
}
