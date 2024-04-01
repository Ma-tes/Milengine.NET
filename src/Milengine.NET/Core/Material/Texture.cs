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
    }

    public unsafe void LoadUnsafeTexture()
    {
        var textureMapper = GraphicsContext.Global.TextureMapper;
        ReadOnlySpan<Rgba32> textureMapData = textureMapper.TextureMapData.Span;

        GraphicsContext.Graphics.TexSubImage2D(textureMapper.TextureType, 0, Position.X,
            Position.Y, (uint)textureMapper.TextureMapSize.X, (uint)textureMapper.TextureMapSize.Y, GLEnum.Rgba, GLEnum.UnsignedByte,
                textureMapData);

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

    public void Dispose() { }
}