using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Milengine.NET.Core.Structures;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Milengine.NET.Core.SceneManager;

//TODO: Add specific tag function, with related reflection/source generator.
public class SceneHolder : IDisposable
{
    internal CancellationTokenSource CancellationTokenSource { get; set; } = new();
    internal ShaderAttacher Shader { get; set; }

    public List<IRenderableObject> RenderableObjects { get; internal set; } = [];
    public MemoryMapper<ICamera> SceneCameraMapper { get; set; }

    public virtual Action<IRenderableObject>? OnObjectInitialization { get; set; }
    public virtual Action<IRenderableObject>? OnObjectUpdate { get; set; }
    public virtual Action<IRenderableObject>? OnObjectRender { get; set; }

    //TODO: Use more specific tag system.
    public ref ICamera CurrentCamera => ref SceneCameraMapper.GetValueReference();
 
    public IWindow Window { get; }

    public TickCounter CurrentFrameTick { get; }

    public SceneHolder(IWindow window, Memory<ICamera> sceneCameras)
    {
        SceneCameraMapper = new MemoryMapper<ICamera>(sceneCameras);
        CurrentFrameTick = new TickCounter(1);
        Window = window;
        RenderableObjects.AddRange(GetRelativeRenderableCameras(sceneCameras.Span.ToArray()));
    }

    public void Initializate()
    {
        Window.Load += ExecuteObjectsInitialization;
        Window.Update += ExecuteObjectsUpdate;
        Window.Render += ExecuteObjectsRender;
    }

    public virtual void ExecuteObjectsInitialization()
    {
        Shader = new ShaderAttacher(GraphicsContext.VertexShader, GraphicsContext.FragmentShader);
        Shader.AttachShaders();

        ExecuteObjectsAction((IRenderableObject currentObject) =>
            currentObject.OnInitialization(), OnObjectInitialization);
    }

    public virtual void ExecuteObjectsUpdate(double deltaTime)
    {
        ExecuteObjectsAction((IRenderableObject currentObject) =>
            currentObject.OnUpdate((float)deltaTime), OnObjectUpdate);

        //float colorIndex = MathF.Abs(MathF.Sin((float)Window.Time / 2) - 0.25f);
        GraphicsContext.Graphics.Uniform3(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "additionalColor"), 1.0f, 1.0f, 1.0f);
            //(colorIndex * colorIndex) - 0.25f, colorIndex + 0.25f, colorIndex + 0.2f);
    }

    public virtual void ExecuteObjectsRender(double deltaTime)
    {
        GraphicsContext.Global.GraphicsBeginFrameRender();
        unsafe
        {
            Matrix4X4<float> currentCameraProjectionView = CurrentCamera.CalculateProjectionView();
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uProjection"),
                1, false, (float*)&currentCameraProjectionView);

            Matrix4X4<float> currentCameraView = CurrentCamera.CalculateCameraView();
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uView"),
                1, false, (float*)&currentCameraView);
            GraphicsContext.Graphics.Uniform1(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "ourTexture"), 0);
            ExecuteObjectsAction((IRenderableObject currentObject) =>
            {
                if(!currentObject.Equals(CurrentCamera))
                {
                    GraphicsContext.Graphics.UseProgram(GraphicsContext.Global.ShaderHandle);
                    currentObject.OnRender((float)deltaTime);
                }
            }, OnObjectUpdate);
        }
    }

    private void ExecuteObjectsAction(Action<IRenderableObject> objectAsyncFunction,
        Action<IRenderableObject>? onObjectAction = null)
    {
        if(RenderableObjects is null || RenderableObjects.Count == 0) { return; }
        int renderableObjectsCount = RenderableObjects.Count;
        for (int i = 0; i < renderableObjectsCount; i++)
        {
            objectAsyncFunction(RenderableObjects[i]);
            if(onObjectAction is not null)
                onObjectAction(RenderableObjects[i]);
        }
    }

    private static IEnumerable<IRenderableObject> GetRelativeRenderableCameras(ICamera[] cameras)
    {
        int camerasLenght = cameras.Length;
        for (int i = 0; i < camerasLenght; i++)
        {
            ICamera currentCamera = cameras[i];
            if(currentCamera.IsRenderable) yield return (IRenderableObject)currentCamera;
        }
    }

    public void Dispose()
    {
        Window.Dispose();
        Shader.DeattachShaders();
    }
}