using System.Drawing;
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
    private static GL currentOpenGLHandler;
    private static IWindow window;

    public static void Main(string[] args)
    {
        window = Window.Create(windowOptions);
        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Run();
        //SceneHolder testScene = new SceneHolder()
    }

    private static void OnLoad()
    {
        currentOpenGLHandler = window.CreateOpenGL();
        currentOpenGLHandler.ClearColor(Color.White);
    }

    private static void OnUpdate(double deltaTime)
    {
    }

    private static void OnRender(double deltaTime)
    {
        currentOpenGLHandler.Clear(ClearBufferMask.ColorBufferBit);
    }
}