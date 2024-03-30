using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Milengine.NET.Core.SceneManager;

//TODO: Add specific tag function, with related reflection/source generator.
public class SceneHolder : IDisposable
{
    internal CancellationTokenSource CancellationTokenSource { get; set; } = new();
    internal Memory<ICamera> SceneCameras { get; set; } = new();

    public List<IRenderableObject> RenderableObjects { get; internal set; } = new();

    public virtual Action<IRenderableObject>? OnObjectInitialization { get; set; }
    public virtual Action<IRenderableObject>? OnObjectUpdate { get; set; }
    public virtual Action<IRenderableObject>? OnObjectRender { get; set; }

    //TODO: Use more specific tag system.
    public int MainCameraIndex { get; set; } = 0;
    public ICamera CurrentCamera => SceneCameras.Span[MainCameraIndex];

    public IWindow Window { get; }

    public TickCounter CurrentFrameTick { get; }

    public SceneHolder(IWindow window, Memory<ICamera> sceneCameras)
    {
        SceneCameras = sceneCameras;
        CurrentFrameTick = new TickCounter();
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
        uint vertexShader = CreateRelativeShader(GraphicsContext.VertexShader, ShaderType.VertexShader);
        uint fragmentShader = CreateRelativeShader(GraphicsContext.FragmentShader, ShaderType.FragmentShader);

        //TODO: Add the dispose of relative shaders.
        GraphicsContext.Global.ShaderHandle = GraphicsContext.Graphics.CreateProgram();
        GraphicsContext.Graphics.AttachShader(GraphicsContext.Global.ShaderHandle, vertexShader);
        GraphicsContext.Graphics.AttachShader(GraphicsContext.Global.ShaderHandle, fragmentShader);
        GraphicsContext.Graphics.LinkProgram(GraphicsContext.Global.ShaderHandle);

        ExecuteObjectsAction((IRenderableObject currentObject) =>
            currentObject.OnInitialization(), OnObjectInitialization);
    }

    public virtual void ExecuteObjectsUpdate(double deltaTime)
    {
        GraphicsContext.Global.GraphicsBeginFrameRender();
        ExecuteObjectsAction((IRenderableObject currentObject) =>
         currentObject.OnUpdate((float)deltaTime), OnObjectUpdate);

        float colorIndex = MathF.Abs(MathF.Sin((float)Window.Time / 2) - 0.25f);
        GraphicsContext.Graphics.Uniform3(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "additionalColor"),
            (colorIndex * colorIndex) - 0.25f, colorIndex + 0.25f, colorIndex + 0.2f);
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
                if(!currentObject.Equals(SceneCameras.Span[MainCameraIndex]))
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

    private static uint CreateRelativeShader(string shaderData, ShaderType shaderType)
    {
        uint shaderHandle = GraphicsContext.Graphics.CreateShader(shaderType);
        GraphicsContext.Graphics.ShaderSource(shaderHandle, shaderData);
        GraphicsContext.Graphics.CompileShader(shaderHandle);
        return shaderHandle;
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

    public void Dispose() { }
}