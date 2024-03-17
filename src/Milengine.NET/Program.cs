using System.Numerics;
using System.Runtime.CompilerServices;
using Milengine.NET.Core;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.SceneManager;
using Milengine.NET.Parser;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Milengine.NET;
public class Program
{
    private static WindowOptions windowOptions = WindowOptions.Default with
    {
        Size = new Vector2D<int>(1024, 720),
        Title = "Window test",
    };
    private static IWindow window;
    private static GraphicsContext graphicsContext;
    private static SceneHolder sceneHolder;

    private static IKeyboard keyboard;

    public static void Main()
    {
        window = Window.Create(windowOptions);
        graphicsContext = new(window);
        GraphicsContext.Global = graphicsContext;
        sceneHolder = new SceneHolder(window, null);

        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Run();
    }

    private static void OnLoad()
    {
        graphicsContext.GraphicsInitialization();
        var inputManager = window.CreateInput();
        keyboard = inputManager.Keyboards[0];

        ObjFormat objectModel = new ObjFormat();
        sceneHolder.RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj")));
        sceneHolder.RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Crate_2.obj")));
        sceneHolder.RenderableObjects[0].Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 120);
        
        sceneHolder.ExecuteObjectsInitialization();
    }

    private static void OnUpdate(double deltaTime)
    {
        graphicsContext.GraphicsBeginFrameRender();
        sceneHolder.ExecuteObjectsUpdate((float)deltaTime);
        if(keyboard.IsKeyPressed(Key.W))
            sceneHolder.RenderableObjects[0].Position += Vector3.UnitZ * 20 * (float)deltaTime;
        if(keyboard.IsKeyPressed(Key.S))
            sceneHolder.RenderableObjects[0].Position -= Vector3.UnitZ * 20 * (float)deltaTime;
        if(keyboard.IsKeyPressed(Key.A))
            sceneHolder.RenderableObjects[0].Position += Vector3.UnitX * 20 * (float)deltaTime;
        if(keyboard.IsKeyPressed(Key.D))
            sceneHolder.RenderableObjects[0].Position -= Vector3.UnitX * 20 * (float)deltaTime;
    }

    private static void OnRender(double deltaTime)
    {
        GraphicsContext.Global.GraphicsBeginFrameRender();
        sceneHolder.ExecuteObjectsRender((float)deltaTime);
    }
}