using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Examples.CoreTest;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Milengine.NET;
public class Program
{
    private static readonly WindowOptions windowOptions = WindowOptions.Default with
    {
        Size = new Vector2D<int>(1024, 720),
        Title = "Window test",
    };

    private static MainScene testScene;

    public static void Main()
    {
        GlfwWindowing.RegisterPlatform();
        GlfwWindowing.Use();

        var currentWindow = Window.Create(windowOptions);

        testScene = new(currentWindow, new Memory<ICamera>([new ViewCamera()]));
        GraphicsContext.Global = new GraphicsContext(testScene.Window);

        testScene.Initializate();
        currentWindow.Run();
    }
}