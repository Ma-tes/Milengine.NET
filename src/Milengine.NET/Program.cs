using System.Numerics;
using Milengine.NET.Core;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.SceneManager;
using Milengine.NET.Parser;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
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

        ObjFormat objectModel = new ObjFormat();
        sceneHolder.RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Podlaha.obj")));
        sceneHolder.RenderableObjects.Add(
            new Model(objectModel.LoadFormatModelData(@"/Users/mates/Downloads/Crate_2.obj")));
        sceneHolder.ExecuteObjectsInitialization();
    }

    private static void OnUpdate(double deltaTime)
    {
        graphicsContext.GraphicsBeginFrameRender();
        sceneHolder.ExecuteObjectsUpdate((float)deltaTime);
    }

    private static void OnRender(double deltaTime)
    {
        sceneHolder.ExecuteObjectsRender((float)deltaTime);
    }
}