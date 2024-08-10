using Milengine.NET.Core.Structures;
using Silk.NET.Windowing;

namespace Milengine.NET.SceneManager;

public class SceneManager : IDisposable
{
    public MemoryMapper<SceneHolder> Scenes { get; set; }
    public IWindow Window { get; }

    public Func<MemoryMapper<SceneHolder>, SceneHolder> OnSceneSwitch { get; set; }
 
    public ref SceneHolder CurrentScene => ref Scenes.GetValueReference();
 
    public SceneManager(IWindow window, params SceneHolder[] scenes)
    {
        Window = window;
        Scenes = new MemoryMapper<SceneHolder>(scenes);
    }

    public void ExecuteSceneManager()
    {
        ref SceneHolder currentScene = ref Scenes.GetValueReference();
        Scenes.Values.Span[0].Initializate();
        Window.Update += OnManagerUpdate;
    }

    protected virtual void OnManagerUpdate(double deltaTime)
    {
        ref SceneHolder previousScene = ref Scenes.GetValueReference();
        SceneHolder currentScene = OnSceneSwitch(Scenes);
        if(previousScene.Equals(currentScene) || currentScene is null) return;
        previousScene.Deinitializate();

        currentScene.Initializate();
        if(!currentScene.IsInitializated)
            currentScene.ExecuteObjectsInitialization();
    }
 
    public void Dispose() { }
}
