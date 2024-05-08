using Milengine.NET.Core.Cameras;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.SceneManager;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using Silk.NET.Input;

namespace Milengine.NET_CreatingScene;
public class Program
{
    private static readonly WindowOptions windowOptions = WindowOptions.Default with
    {
        Size = new Vector2D<int>(1200, 600),
        Title = "Window test",
        PreferredDepthBufferBits = 24
    };

    public static void Main()
    {
        GlfwWindowing.RegisterPlatform();
        GlfwWindowing.Use();

        var currentWindow = Window.Create(windowOptions);
 
        SceneManager sceneManager = new SceneManager(currentWindow,
            new MainScene(currentWindow, new Memory<ICamera>([
                new ViewCamera(),
                new ViewCamera() { Position = 20 * Vector3D<float>.UnitY}
            ])),
            new SecondScene(currentWindow, new Memory<ICamera>([
                new ViewCamera(),
                new SceneCamera()
            ]))
        );
        GraphicsContext.Global = new GraphicsContext(currentWindow)
        {
            RelativeResolution = new Vector2D<uint>(480, 240)
        };

        sceneManager.OnSceneSwitch = (scenes) =>
        {
            var inputManager = currentWindow.CreateInput();
            var keyboard = inputManager.Keyboards[0];
            if(keyboard.IsKeyPressed(Key.Number3))
            {
                Console.WriteLine("Number 3 is pressed");
                scenes.MapIndex = (scenes.MapIndex + 1) % (scenes.Values.Length);
            }
            return scenes.GetValueReference();
        };
        sceneManager.ExecuteSceneManager();
        currentWindow.Run();
    }
}
