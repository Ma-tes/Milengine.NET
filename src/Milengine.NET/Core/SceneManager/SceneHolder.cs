using System.Collections.Concurrent;
using System.Numerics;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
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

    public int MainCameraIndex { get; set; } = 0;
    public IWindow Window { get; }

    public TickCounter CurrentFrameTick { get; }

    public SceneHolder(IWindow window, Memory<ICamera> sceneCameras)
    {
        SceneCameras = sceneCameras;
        CurrentFrameTick = new TickCounter();
        Window = window;
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

    public virtual void ExecuteObjectsUpdate(float deltaTime)
    {
        ExecuteObjectsAction((IRenderableObject currentObject) =>
         currentObject.OnUpdate(deltaTime), OnObjectUpdate);

        float colorIndex = MathF.Abs(MathF.Sin((float)Window.Time / 2) - 0.25f);
        GraphicsContext.Graphics.Uniform3(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "additionalColor"),
            (colorIndex * colorIndex) - 0.25f, colorIndex + 0.25f, colorIndex + 0.2f);
    }

    public virtual void ExecuteObjectsRender(float deltaTime)
    {
        Matrix4x4 cameraView = Matrix4x4.CreateLookAt(
            new Vector3(0.0f, 0.0f, 30f),
            new Vector3(0.0f, 0.0f, 29f),
            Vector3.UnitY
        );
        Matrix4x4 projectionView = Matrix4x4.CreatePerspectiveFieldOfView(
            MathF.PI * 45.0f / 180.0f, Window.FramebufferSize.X / Window.FramebufferSize.Y, 0.1f, 100.0f
        );

        unsafe
        {
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uView"),
                1, false, (float*)&cameraView);
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uProjection"),
                1, false, (float*)&projectionView);
            ExecuteObjectsAction((IRenderableObject currentObject) =>
            {
                GraphicsContext.Graphics.UseProgram(GraphicsContext.Global.ShaderHandle);
                currentObject.OnRender(deltaTime);
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
            objectAsyncFunction(RenderableObjects[i]);//.ConfigureAwait(true);
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

    public void Dispose() { }
}