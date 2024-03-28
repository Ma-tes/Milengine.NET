using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace Milengine.NET.Core.Graphics;

public class GraphicsContext : IDisposable
{
    public const string VertexShader = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec3 vPos;
        layout (location = 1) in vec3 aColor;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;
        out vec3 ourColor;
        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(vPos.x, vPos.y, vPos.z, 1.0);
            ourColor = aColor;
        }
    ";

    public const string FragmentShader = @"
        #version 330 core
        out vec4 FragColor;
        in vec3 ourColor;

        uniform vec3 additionalColor;
        void main()
        {
            FragColor = vec4(vec3(dot(ourColor + additionalColor, vec3(.5, .5, .5))), 1.0f);
        }
    ";

    public static GL Graphics { get; internal set; } = null!;

    public IWindow Window { get; set; }
    public Glfw WindowGlfw { get; set; }

    public uint ShaderHandle { get; set; }
    public Vector2D<uint> RelativeResolution { get; set; }

    public PolygonMode CurrentRenderingType { get; set; } =
        PolygonMode.Fill;
    public static GraphicsContext Global { get; internal set; } = null!;

    public GraphicsContext(IWindow window)
    {
        Window = window;
        WindowGlfw = GlfwWindowing.GetExistingApi(Window)!;
        RelativeResolution = new Vector2D<uint>(
            (uint)Window.Size.X, (uint)Window.Size.Y);
    }

    public virtual void GraphicsInitialization()
    {
        Graphics ??= Window.CreateOpenGL();
        Graphics.Enable(GLEnum.LineSmooth);
    }

    public virtual void GraphicsBeginFrameRender()
    {
        Graphics.DepthFunc(DepthFunction.Lequal);
        Graphics.Enable(
            GLEnum.DepthTest |
            GLEnum.Blend |
            GLEnum.CullFace
            );
        Graphics.FrontFace(FrontFaceDirection.Ccw); //Counter clock wise.
        Clear(
            InlineValueParameter_Three<ClearBufferMask>.CreateInstance(
                ClearBufferMask.ColorBufferBit,
                ClearBufferMask.DepthBufferBit,
                ClearBufferMask.StencilBufferBit
            )
        );
        Graphics.Viewport(0, 0, RelativeResolution.X, RelativeResolution.Y);
    }

    public static void SetVertexAttributePointer(uint index, int typeCount,
        VertexAttribPointerType type, uint vertexSize, int offset)
    {
        unsafe { Graphics.VertexAttribPointer(index,
            typeCount, type, false, vertexSize, (void*)offset); }
        Graphics.EnableVertexAttribArray(index);
    }

    private void Clear(InlineParameter_Three<ClearBufferMask> bufferMask)
    {
        Graphics.ClearColor(0, 0, 0, 1);
        Graphics.Clear(bufferMask[0] | bufferMask[1] | bufferMask[2]);
    }

    public void Dispose() { Graphics!.Dispose(); }
}