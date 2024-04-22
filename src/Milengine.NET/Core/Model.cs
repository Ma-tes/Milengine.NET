using System.Numerics;
using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Interfaces;
using Silk.NET.Maths;

namespace Milengine.NET.Core;

public class Model : IRenderableObject
{
    //TODO: Consider separating related data, into
    //transform structure.
    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
    public Quaternion<float> Rotation { get; set; } = Quaternion<float>.Identity;
    public float Scale { get; set; } = 1.0f;
    public Matrix4x4 ViewMatrix =>
        Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(new Quaternion(Rotation.X, Rotation.Y, Rotation.Z, Rotation.W))
        * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(new Vector3(Position.X, Position.Y, Position.Z));

    public ReadOnlyMemory<GraphicsMesh> Meshes { get; protected set; }
    public Texture TextureTemporaryHolder { get; set; }

    public Model(ReadOnlyMemory<GraphicsMesh> meshes)
    {
        Meshes = meshes;
    }

    public virtual void OnInitialization()
    {
        int meshesLength = Meshes.Length;
        var meshesSpan = Meshes.Span;
        //GraphicsContext.Global.TextureMapper.Bind();
        for (int i = 0; i < meshesLength; i++) { meshesSpan[i].LoadMesh(); }
    }

    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnRender(float deltaTime)
    {
        unsafe
        {
            Matrix4x4 currentModelMatrix = ViewMatrix;
            GraphicsContext.Graphics.UniformMatrix4(GraphicsContext.Graphics.GetUniformLocation(GraphicsContext.Global.ShaderHandle, "uModel"),
                1, false, (float*)&currentModelMatrix);
        }
        int meshesLength = Meshes.Length;
        for (int i = 0; i < meshesLength; i++)
        {
            Meshes.Span[i].VertexArrayBuffer.Bind(); 
            TextureTemporaryHolder.LoadUnsafeTexture();
            TextureTemporaryHolder.Bind();
            unsafe { GraphicsContext.Graphics.DrawArrays(GraphicsContext.Global.CurrentRenderingType, 0, (uint)Meshes.Span[i].Indices.Buffer.Length / 3); }
        }
    }
}