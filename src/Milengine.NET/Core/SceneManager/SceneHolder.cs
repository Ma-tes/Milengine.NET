using System.Collections.Concurrent;
using Milengine.NET.Core.Interfaces;
using SharpBgfx;
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

    public SceneHolder(Memory<ICamera> sceneCameras, ref ConcurrentBag<IRenderableObject> renderableObjects)
    {
        SceneCameras = sceneCameras;
        RenderableObjects = renderableObjects;
        CurrentFrameTick = new TickCounter();
    }

    public async ValueTask ExecuteObjectsInitializationAsync(WindowOptions windowOptions,
        CancellationToken cancellationToken)
    {
        Bgfx.Init();
        Bgfx.Reset(windowOptions.Size.X, windowOptions.Size.Y, ResetFlags.None);
        await ExecuteObjectsActionAsync((IRenderableObject currentObject) =>
         currentObject.OnInitializationAsync(), cancellationToken, OnObjectInitialization);
    }

    public virtual async ValueTask ExecuteObjectsUpdateAsync(float deltaTime, CancellationToken cancellationToken)
    {
        
        await ExecuteObjectsActionAsync((IRenderableObject currentObject) =>
         currentObject.OnUpdateAsync(deltaTime), cancellationToken, OnObjectUpdate);
    }

    private async ValueTask ExecuteObjectsActionAsync(Func<IRenderableObject, Task> objectAsyncFunction,
        CancellationToken cancellationToken, Action<IRenderableObject>? onObjectAction = null)
    {
        if(cancellationToken.IsCancellationRequested || 
            RenderableObjects is null || RenderableObjects.Count == 0) { return; }

        while(RenderableObjects.TryPeek(out IRenderableObject? renderableObject))
        {
            await objectAsyncFunction(renderableObject).ConfigureAwait(true);
            if(onObjectAction is not null)
                onObjectAction(renderableObject);
        }
    }

    public void Dispose() { }
}
