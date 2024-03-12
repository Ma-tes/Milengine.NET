using System.Drawing;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Structures;
using Milengine.NET.Core.SceneManager;
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

    private static GraphicsBufferData<float> vertices;
    private static GraphicsBufferData<uint> indices;
    private static VertexArrayBuffer<float, uint> vertexArrayBuffer;

    private static uint shaderHandle;
    private static TickCounter tickCounter;

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
        tickCounter = new TickCounter(1);
        vertices =
            new GraphicsBufferData<float>(new ReadOnlyMemory<float>(
                //X, Y, Z, R, G, B
                [0.5f,  0.5f, 0.0f, 0.0f, 0.0f, 0.0f,
                0.5f, -0.5f, 0.0f,  0.0f, 0.0f, 0.0f,
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f,
                -0.5f,  0.5f, 0.5f, 0.0f, 0.0f, 0.0f]), BufferTargetARB.ArrayBuffer);
        indices =
            new GraphicsBufferData<uint>(new ReadOnlyMemory<uint>(
                [0, 1, 3,
                 1, 2, 3]), BufferTargetARB.ElementArrayBuffer);

        vertexArrayBuffer = new VertexArrayBuffer<float, uint>(
            vertices, indices
        );
        uint vertexShader = CreateRelativeShader(VertexShader, ShaderType.VertexShader);
        uint fragmentShader = CreateRelativeShader(FragmentShader, ShaderType.FragmentShader);

        shaderHandle = GraphicsContext.Graphics.CreateProgram();
        GraphicsContext.Graphics.AttachShader(shaderHandle, vertexShader);
        GraphicsContext.Graphics.AttachShader(shaderHandle, fragmentShader);
        GraphicsContext.Graphics.LinkProgram(shaderHandle);
        //TODO: Add the dispose of relative shaders.

        GraphicsContext.SetVertexAttributePointer<float>(0, 3, VertexAttribPointerType.Float, 6, 0);
        GraphicsContext.SetVertexAttributePointer<float>(1, 3, VertexAttribPointerType.Float, 6, 3);
    }

    private static void OnUpdate(double deltaTime)
    {
        float relativeColorIndex = Math.Abs(MathF.Sin(tickCounter.CalculateRelativeTimeTick()) * ((float)deltaTime * 50));
        
        Console.WriteLine(relativeColorIndex);
        GraphicsContext.Graphics.Uniform3(GraphicsContext.Graphics.GetUniformLocation(shaderHandle, "additionalColor"),
            relativeColorIndex, 1.0f, 1.0f);
    }

    private static void OnRender(double deltaTime)
    {
        graphicsContext.GraphicsBeginFrameRender();
        vertexArrayBuffer.Bind();

        GraphicsContext.Graphics.UseProgram(shaderHandle);
        unsafe { GraphicsContext.Graphics.DrawElements(PrimitiveType.Triangles, (uint)indices.Buffer.Length, GLEnum.UnsignedInt, null); }
        //GraphicsContext.Graphics.DrawArrays(GLEnum.Triangles, 0, 3);
    }

    private static uint CreateRelativeShader(string shaderData, ShaderType shaderType)
    {
        uint shaderHandle = GraphicsContext.Graphics.CreateShader(shaderType);
        GraphicsContext.Graphics.ShaderSource(shaderHandle, shaderData);
        GraphicsContext.Graphics.CompileShader(shaderHandle);
        return shaderHandle;
    }
}