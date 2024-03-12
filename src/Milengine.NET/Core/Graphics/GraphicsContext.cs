using System.Numerics;
using System.Runtime.InteropServices;
using Milengine.NET.Core.SceneManager;
using Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Milengine.NET.Core.Graphics;

public class GraphicsContext : IDisposable
{
    public static GL Graphics { get; internal set; } = null!;

    public IWindow Window { get; set; }
    public Vector2D<uint> RelativeResolution { get; set; }

    public PolygonMode CurrentRenderingType { get; set; } =
        PolygonMode.Fill;

    public GraphicsContext(IWindow window)
    {
        Window = window;
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
