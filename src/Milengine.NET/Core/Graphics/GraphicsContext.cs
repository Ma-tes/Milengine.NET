using Milengine.NET.Core.Material;
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
        layout (location = 2) in vec2 aTextureCoordinates;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

        out vec3 ourColor;
        out vec2 ourTextureCoordinates;
        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(vPos.x, vPos.y, vPos.z, 1.0);
            ourColor = aColor;
            ourTextureCoordinates = aTextureCoordinates;
        }
    ";

    public const string FragmentShader = @"
        #version 330 core
        out vec4 FragColor;
        in vec3 ourColor;
        in vec2 ourTextureCoordinates;

        uniform vec3 additionalColor;
        uniform sampler2D ourTexture;
        void main()
        {
            FragColor = texture(ourTexture, ourTextureCoordinates) * vec4(vec3(dot(ourColor + additionalColor, vec3(.5, .5, .5))), 1.0f);
        }
    ";

    public static GL Graphics { get; internal set; } = null!;

    public IWindow Window { get; set; }
    public Glfw WindowGlfw { get; set; }

    public uint ShaderHandle { get; set; }
    public Vector2D<uint> RelativeResolution { get; set; }

    public GLEnum CurrentRenderingType { get; set; } = GLEnum.Triangles;

    public TextureMapper TextureMapper { get; set; } = null!;

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
        Graphics.Enable(
            GLEnum.DepthTest |
            GLEnum.Blend |
            GLEnum.CullFace
            );
        Graphics.FrontFace(FrontFaceDirection.Ccw); //Counter clock wise.
    }

    public virtual void GraphicsBeginFrameRender()
    {
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

    public void Dispose()
    {
        Graphics!.Dispose();
        Window.Dispose();
        WindowGlfw.Dispose();
    }
}