using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core;

public class Model : IRenderableObject
{
    //TODO: Consider separating related data, into
    //transform structure.
    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
    public Quaternion<float> Rotation { get; set; } = Quaternion<float>.Identity;
    public float Scale { get; set; } = 1.0f;
    public Matrix4X4<float> ViewMatrix =>
        Matrix4X4<float>.Identity * Matrix4X4.CreateFromQuaternion(Rotation)
        * Matrix4X4.CreateScale(Scale) * Matrix4X4.CreateTranslation(Position);

    public ReadOnlyMemory<GraphicsMesh> Meshes { get; protected set; }

    public Model(ReadOnlyMemory<GraphicsMesh> meshes)
    {
        Meshes = meshes;
    }

    public virtual void OnInitialization()
    {
        int meshesLength = Meshes.Length;
        var meshesSpan = Meshes.Span;
        for (int i = 0; i < meshesLength; i++) { meshesSpan[i].LoadMesh(); }
    }

    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnRender(float deltaTime)
    {
        unsafe
        {
            Matrix4X4<float> currentModelMatrix = ViewMatrix;
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uModel"),
                1, false, (float*)&currentModelMatrix);
        }
        int meshesLength = Meshes.Length;
        for (int i = 0; i < meshesLength; i++)
        {
            Meshes.Span[i].VertexArrayBuffer.Bind(); 
            unsafe { GraphicsContext.Graphics.DrawArrays(GLEnum.Triangles, 0, (uint)Meshes.Span[i].Vertices.Buffer.Length); }
        }
    }
}
