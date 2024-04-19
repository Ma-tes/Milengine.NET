using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Examples;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Milengine.NET;
public class Program
{
    private static readonly WindowOptions windowOptions = WindowOptions.Default with
    {
        Size = new Vector2D<int>(1200, 600),
        Title = "Window test",
    };

    public static void Main()
    {
        GlfwWindowing.RegisterPlatform();
        GlfwWindowing.Use();

        var currentWindow = Window.Create(windowOptions);
 
        MainScene testScene = new(currentWindow, new Memory<ICamera>([
            new ViewCamera(),
            new ViewCamera() { Position = 20 * Vector3D<float>.UnitY}
        ]));
        GraphicsContext.Global = new GraphicsContext(testScene.Window)
        {
            RelativeResolution = new Vector2D<uint>(480, 240)
        };

        testScene.Initializate();
        currentWindow.Run();
    }
}