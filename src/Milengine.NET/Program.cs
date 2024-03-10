using Milengine.NET.Core.SceneManager;
using Silk.NET.Maths;
using Silk.NET.Windowing;

var windowOptions = WindowOptions.Default with
{
    Size = new Vector2D<int>(1024, 720),
    Title = "Window test",
};
IWindow relativeWindow = Window.Create(windowOptions);
SceneHolder testScene = new SceneHolder()




