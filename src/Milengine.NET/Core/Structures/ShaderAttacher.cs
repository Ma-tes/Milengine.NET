using Milengine.NET.Core.Graphics;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core.Structures;

public readonly struct ShaderAttacher : IDisposable
{
    public uint VertexHandle { get; }
    public uint FragmentHandle { get; }

    public ShaderAttacher(string vertexData, string fragmentData)
    {
        VertexHandle = CreateRelativeShader(vertexData, ShaderType.VertexShader);
        FragmentHandle = CreateRelativeShader(fragmentData, ShaderType.FragmentShader);
    }

    public void AttachShaders()
    {
        GraphicsContext.Global.ShaderHandle = GraphicsContext.Graphics.CreateProgram();
        GraphicsContext.Graphics.AttachShader(GraphicsContext.Global.ShaderHandle, VertexHandle);
        GraphicsContext.Graphics.AttachShader(GraphicsContext.Global.ShaderHandle, FragmentHandle);
        GraphicsContext.Graphics.LinkProgram(GraphicsContext.Global.ShaderHandle);
    }

    public void DeattachShaders()
    {
        GraphicsContext.Graphics.DetachShader(GraphicsContext.Global.ShaderHandle, VertexHandle);
        GraphicsContext.Graphics.DetachShader(GraphicsContext.Global.ShaderHandle, FragmentHandle);
        GraphicsContext.Graphics.DeleteProgram(GraphicsContext.Global.ShaderHandle);
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
