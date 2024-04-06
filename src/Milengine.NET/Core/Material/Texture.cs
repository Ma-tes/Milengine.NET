using Milengine.NET.Core.Graphics;
using Milengine.NET.Core.Graphics.Interfaces;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
        RenderParameters.Push(
            new TextureRenderParameter(GLEnum.TextureWrapS, (float)GLEnum.Repeat)
        );
        RenderParameters.Push(
            new TextureRenderParameter(GLEnum.TextureWrapT, (float)GLEnum.Repeat)
        );
    }

    public unsafe void LoadUnsafeTexture()
    {
        var textureMapper = GraphicsContext.Global.TextureMapper;
        var textureMapData = textureMapper.RelativeTextureMapData.Span;

        int textureWidth = textureMapper.TextureMapSize.X;
        int textureHeight = textureMapper.TextureMapSize.Y;
        for (int i = 0; i < textureHeight; i++)
        {
            int relativePositionX = Position.X / textureWidth
                * textureWidth * textureHeight + (textureWidth * i);
            GraphicsContext.Graphics.TexSubImage2D(
                textureMapper.TextureType, 0,
                0,
                i,
                (uint)textureWidth, 1,
                GLEnum.Rgba,
                GLEnum.UnsignedByte,
                textureMapData[relativePositionX..(relativePositionX + textureWidth)]);
        }

        while(RenderParameters.TryPop(out TextureRenderParameter parameter))
        {
            //GraphicsContext.Graphics.TexParameter(textureMapper.TextureType, parameter.Name, parameter.Value);
        }
        //GraphicsContext.Graphics.TexParameter(textureMapper.TextureType, GLEnum.TextureWrapS, (uint)GLEnum.Repeat);
        //GraphicsContext.Graphics.TexParameter(textureMapper.TextureType, GLEnum.TextureWrapT, (uint)GLEnum.Repeat);
        //GraphicsContext.Graphics.TexParameter(textureMapper.TextureType, GLEnum.TextureWrapR, (uint)GLEnum.Repeat);
        GraphicsContext.Graphics.GenerateMipmap(textureMapper.TextureType);
    }

    public void Bind()
    {
        GraphicsContext.Graphics.ActiveTexture(GLEnum.Texture0);
        GraphicsContext.Graphics.BindTexture(GraphicsContext.Global.TextureMapper.TextureType, Handle);
    }

    public void Dispose() { }
}