using System.Collections.Concurrent;
using Milengine.NET.Core.Interfaces;
using Silk.NET.Windowing;

namespace Milengine.NET.Core.SceneManager;

//TODO: Add specific tag function, with related reflection/source generator.
public class SceneHolder : IDisposable
{
    internal CancellationTokenSource CancellationTokenSource { get; set; } = new();
    internal Memory<ICamera> SceneCameras { get; set; } = new();

    public ConcurrentBag<IRenderableObject> RenderableObjects { get; internal set; } = new();

    public virtual Action<IRenderableObject>? OnObjectInitialization { get; set; }
    public virtual Action<IRenderableObject>? OnObjectUpdate { get; set; }

    public int MainCameraIndex { get; set; } = 0;

    public TickCounter CurrentFrameTick { get; }

    public SceneHolder(Memory<ICamera> sceneCameras, ConcurrentBag<IRenderableObject> renderableObjects)
    {
        SceneCameras = sceneCameras;
        RenderableObjects = renderableObjects;
        CurrentFrameTick = new TickCounter();
    }

    public void ExecuteObjectsInitializationAsync()
    {
        ExecuteObjectsActionAsync((IRenderableObject currentObject) =>
         currentObject.OnInitialization(), OnObjectInitialization);
    }

    public void ExecuteObjectsUpdateAsync(float deltaTime)
    {
        ExecuteObjectsActionAsync((IRenderableObject currentObject) =>
         currentObject.OnUpdate(deltaTime), OnObjectUpdate);
    }

    private void ExecuteObjectsActionAsync(Action<IRenderableObject> objectAsyncFunction,
        Action<IRenderableObject>? onObjectAction = null)
    {
        if(RenderableObjects is null || RenderableObjects.Count == 0) { return; }
        while(RenderableObjects.TryPeek(out IRenderableObject? renderableObject))
        {
            objectAsyncFunction(renderableObject);//.ConfigureAwait(true);
            if(onObjectAction is not null)
                onObjectAction(renderableObject);
        }
    }

    public void Dispose() { }
}