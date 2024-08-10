using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Interfaces;
using Milengine.NET.Core.Material;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Milengine.NET.Core;

public readonly struct TextureRenderParameter
{
    public GLEnum Name { get; }
    public float Value { get; }

    public TextureRenderParameter(GLEnum name, float value)
    {
        Name = name;
        Value = value;
    }
}

public struct Texture : IGraphicsBindable
{
    public string Identificator { get; }
    public Vector2D<int> Position { get; }
 
    public uint Handle { get; }

    public Stack<TextureRenderParameter> RenderParameters { get; set; } = new();

    public Texture(string identificator, Vector2D<int> position)
    {
        Identificator = identificator;
        Position = position;
    }

    public unsafe void LoadUnsafeTexture()
    {
        TextureMapper textureMapper = GraphicsContext.Global.TextureMapper;
        var textureMapData = textureMapper.RelativeTextureMapData.Span;

        int textureWidth = textureMapper.TextureMapSize.X;
        int textureHeight = textureMapper.TextureMapSize.Y;
        for (int i = 0; i < textureHeight; i++)
        {
            int relativePositionX = Position.X / textureWidth
                * textureWidth * textureHeight + (textureWidth * i);
            GraphicsContext.Graphics.TexSubImage2D(textureMapper.TextureType, 0, 0, i,
                (uint)textureWidth, 1,
                GLEnum.Rgba,
                GLEnum.UnsignedByte,
                textureMapData[relativePositionX..(relativePositionX + textureWidth)]);
        }

        while(RenderParameters.TryPop(out TextureRenderParameter parameter))
        {
            GraphicsContext.Graphics.TexParameter(textureMapper.TextureType, parameter.Name, parameter.Value);
        }
        GraphicsContext.Graphics.GenerateMipmap(textureMapper.TextureType);
    }

    public void Bind()
    {
        GraphicsContext.Graphics.ActiveTexture(GLEnum.Texture0);
        GraphicsContext.Graphics.BindTexture(GraphicsContext.Global.TextureMapper.TextureType, Handle);
    }

    public void Dispose()
    {
        GraphicsContext.Graphics.DeleteTexture(Handle);
    }
}