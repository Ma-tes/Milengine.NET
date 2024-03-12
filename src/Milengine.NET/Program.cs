using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Structures;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Milengine.NET;
public class Program
{
    private static WindowOptions windowOptions = WindowOptions.Default with
    {
        Size = new Vector2D<int>(1024, 720),
        Title = "Window test",
    };

    private static IWindow window;
    private static GraphicsContext graphicsContext;

    private const string VertexShader = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec3 vPos;
        layout (location = 1) in vec3 aColor;

        out vec3 ourColor;
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
            ourColor = aColor;
        }
    ";

    private const string FragmentShader = @"
        #version 330 core
        out vec4 FragColor;
        in vec3 ourColor;

        uniform vec3 additionalColor;
        void main()
        {
            FragColor = vec4(ourColor + additionalColor, 1.0f);
        }
    ";

    private static uint shaderHandle;
    private static GraphicsMesh graphicsMesh;

    public static void Main()
    {
        window = Window.Create(windowOptions);
        graphicsContext = new(window);

        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Run();
    }

    private static void OnLoad()
    {
        graphicsContext.GraphicsInitialization();
        graphicsMesh = new GraphicsMesh(
            new ReadOnlyMemory<Vertex<float>>([
                new Vertex<float>(new(0.5f, 0.5f, 0.0f), new(1.0f, 0.0f, 0.0f), new(1.0f, 1.0f)),
                new Vertex<float>(new(0.5f, -0.5f, 0.0f), new(0.0f, 1.0f, 0.0f), new(1.0f, 0.0f)),
                new Vertex<float>(new(-0.5f, -0.5f, 0.0f), new(0.0f, 0.0f, 1.0f), new(0.0f, 0.0f)),
                new Vertex<float>(new(-0.5f, 0.5f, 0.5f), new(1.0f, 1.0f, 0.0f), new(0.0f, 1.0f))
            ]),
            new ReadOnlyMemory<uint>([
                0, 1, 3,
                1, 2, 3
            ]));

        uint vertexShader = CreateRelativeShader(VertexShader, ShaderType.VertexShader);
        uint fragmentShader = CreateRelativeShader(FragmentShader, ShaderType.FragmentShader);

        //TODO: Add the dispose of relative shaders.
        shaderHandle = GraphicsContext.Graphics.CreateProgram();
        GraphicsContext.Graphics.AttachShader(shaderHandle, vertexShader);
        GraphicsContext.Graphics.AttachShader(shaderHandle, fragmentShader);
        GraphicsContext.Graphics.LinkProgram(shaderHandle);

        graphicsMesh.LoadMesh();
    }

    private static void OnUpdate(double deltaTime)
    {
        GraphicsContext.Graphics.Uniform3(GraphicsContext.Graphics.GetUniformLocation(shaderHandle, "additionalColor"),
            0.0f, 0.0f, 1.0f);
    }

    private static void OnRender(double deltaTime)
    {
        graphicsContext.GraphicsBeginFrameRender();
        graphicsMesh.VertexArrayBuffer.Bind();

        GraphicsContext.Graphics.UseProgram(shaderHandle);
        unsafe { GraphicsContext.Graphics.DrawElements(PrimitiveType.Triangles, (uint)graphicsMesh.Indices.Buffer.Length, GLEnum.UnsignedInt, null); }
    }

    private static uint CreateRelativeShader(string shaderData, ShaderType shaderType)
    {
        uint shaderHandle = GraphicsContext.Graphics.CreateShader(shaderType);
        GraphicsContext.Graphics.ShaderSource(shaderHandle, shaderData);
        GraphicsContext.Graphics.CompileShader(shaderHandle);
        return shaderHandle;
    }
}