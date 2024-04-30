using Milengine.NET.Core.Structures;
using Silk.NET.Windowing;

namespace Milengine.NET.Core.SceneManager;

public struct SceneManager : IDisposable
{
    public MemoryMapper<SceneHolder> Scenes { get; set; }
    public IWindow Window { get; }

    public Action<SceneHolder> OnSceneInitialization { get; set; }
    public Action<SceneHolder> OnSceneUpdate { get; set; }

    public SceneManager(IWindow window, params SceneHolder[] scenes)
    {
        Window = window;
        Scenes = new MemoryMapper<SceneHolder>(scenes);
    }

    public void ExecuteSceneManager()
    {
    }
 
    public void Dispose() { }
}
